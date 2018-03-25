using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Exomia.Configuration
{
    /// <summary>
    ///     implements <see cref="IConfig" /> interface.
    /// </summary>
    public abstract class ConfigBase : IConfig
    {
        private readonly Dictionary<string, ValueCommentPair> _vcPairs;

        /// <summary>
        ///     string
        /// </summary>
        protected string _comment;

        /// <summary>
        ///     IConfigSource
        /// </summary>
        protected IConfigSource _configSource;

        /// <summary>
        ///     string
        /// </summary>
        protected string _name;

        /// <summary>
        ///     ConfigBase constructor
        /// </summary>
        public ConfigBase(IConfigSource configSource, string name, string comment = "")
        {
            _name = name;
            _configSource = configSource;
            _comment = comment;

            _vcPairs = new Dictionary<string, ValueCommentPair>();
        }

        internal Dictionary<string, ValueCommentPair> VCPairs
        {
            get { return _vcPairs; }
        }

        /// <summary>
        ///     <see cref="IConfig.KeySet" />
        /// </summary>
        public event ConfigKeyEventHandler KeySet;

        /// <summary>
        ///     <see cref="IConfig.KeyRemoved" />
        /// </summary>
        public event ConfigKeyEventHandler KeyRemoved;

        /// <summary>
        ///     IConfigSource
        /// </summary>
        public IConfigSource ConfigSource
        {
            get { return _configSource; }
        }

        /// <summary>
        ///     string
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        ///     string
        /// </summary>
        public string Comment
        {
            get { return _comment; }
            set { _comment = value; }
        }

        /// <summary>
        ///     <see cref="IConfig.Keys" />
        /// </summary>
        public IEnumerable<string> Keys
        {
            get { return _vcPairs.Keys; }
        }

        /// <summary>
        ///     <see cref="IConfig.Keys" />
        /// </summary>
        public bool Contains(string key)
        {
            return _vcPairs.ContainsKey(key);
        }

        /// <summary>
        ///     <see cref="IConfig" />
        /// </summary>
        public virtual string this[string key]
        {
            get { return _vcPairs[key].Value; }
            set { Set(key, value); }
        }

        /// <summary>
        ///     <see cref="IConfig.Set{T}(string, T, string)" />
        /// </summary>
        public virtual void Set<T>(string key, T value, string comment = "") where T : IConvertible
        {
            ValueCommentPair buffer = new ValueCommentPair(value.ToString(), comment);
            _vcPairs[key] = buffer;

            OnKeySet(this, key, buffer.Value, buffer.Comment);
            KeySet?.Invoke(this, key, buffer.Value, buffer.Comment);
        }

        /// <summary>
        ///     <see cref="IConfig.TrySet{T}(string, T, string)" />
        /// </summary>
        public virtual bool TrySet<T>(string key, T value, string comment = "") where T : IConvertible
        {
            if (_vcPairs.ContainsKey(key)) { return false; }
            Set(key, value, comment);
            return true;
        }

        /// <summary>
        ///     <see cref="IConfig.SetExpanded(string, string, string, string[])" />
        /// </summary>
        public virtual void SetExpanded(string key, string format, string comment, params string[] keys)
        {
            if (comment == null)
            {
                comment = string.Empty;
            }

            Set(key, string.Format(format, keys.Select(x => "${" + x + "}").ToArray()), comment);
        }

        /// <summary>
        ///     <see cref="IConfig.TrySetExpanded(string, string, string, string[])" />
        /// </summary>
        public virtual bool TrySetExpanded(string key, string format, string comment, params string[] keys)
        {
            if (comment == null)
            {
                comment = string.Empty;
            }

            return TrySet(key, string.Format(format, keys.Select(x => "${" + x + "}").ToArray()), comment);
        }

        /// <summary>
        ///     <see cref="IConfig.Get{T}(string)" />
        /// </summary>
        public virtual T Get<T>(string key) where T : IConvertible
        {
            Type type = typeof(T);
            if (type != typeof(string) && !type.IsPrimitive) { return default(T); }

            return (T)Convert.ChangeType(_vcPairs[key].Value, type);
        }

        /// <summary>
        ///     <see cref="IConfig.GetExpanded{T}(string)" />
        /// </summary>
        public virtual T GetExpanded<T>(string key) where T : IConvertible
        {
            Type type = typeof(T);
            if (type != typeof(string) && !type.IsPrimitive) { return default(T); }

            return (T)Convert.ChangeType(ExpandValue(_vcPairs[key].Value), type);
        }

        /// <summary>
        ///     <see cref="IConfig.TryGet{T}(string, out T)" />
        /// </summary>
        public virtual bool TryGet<T>(string key, out T outValue) where T : IConvertible
        {
            outValue = default(T);
            Type type = typeof(T);
            if (type == typeof(string) && !type.IsPrimitive && !_vcPairs.ContainsKey(key)) { return false; }

            try
            {
                outValue = (T)Convert.ChangeType(_vcPairs[key].Value, type);
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        ///     <see cref="IConfig.TryGetExpanded{T}(string, out T)" />
        /// </summary>
        public virtual bool TryGetExpanded<T>(string key, out T outValue) where T : IConvertible
        {
            outValue = default(T);
            Type type = typeof(T);
            if (type == typeof(string) && !type.IsPrimitive && !_vcPairs.ContainsKey(key)) { return false; }

            try
            {
                outValue = (T)Convert.ChangeType(ExpandValue(_vcPairs[key].Value), type);
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        ///     <see cref="IConfig.Remove(string)" />
        /// </summary>
        public virtual bool Remove(string key)
        {
            ValueCommentPair buffer = _vcPairs[key];

            if (_vcPairs.Remove(key))
            {
                OnKeyRemove(this, key, buffer.Value, buffer.Comment);
                KeyRemoved?.Invoke(this, key, buffer.Value, buffer.Comment);
                return true;
            }
            return false;
        }

        /// <summary>
        ///     <see cref="IConfig.TryRemove(string)" />
        /// </summary>
        public virtual bool TryRemove(string key)
        {
            if (_vcPairs.ContainsKey(key)) { return false; }

            return Remove(key);
        }

        /// <summary>
        ///     ExpandValue
        /// </summary>
        protected string ExpandValue(string value)
        {
            Match match = null;
            while ((match = Regex.Match(value, @"\${(.+?)(?:\.(.+?))?\}")).Success)
            {
                if (match.Groups[2].Success && match.Groups[1].Success)
                {
                    IConfig cfg = _configSource.Get(match.Groups[1].Value);
                    value = value.Replace(match.Groups[0].Value, cfg.GetExpanded<string>(match.Groups[2].Value));
                }
                else if (match.Groups[1].Success)
                {
                    value = value.Replace(match.Groups[0].Value, GetExpanded<string>(match.Groups[1].Value));
                }
            }
            return value;
        }

        /// <summary>
        ///     called than IConfig set a key
        /// </summary>
        /// <param name="sender">IConfig</param>
        /// <param name="key">new key</param>
        /// <param name="value">new value</param>
        /// <param name="comment">new comment</param>
        protected virtual void OnKeySet(IConfig sender, string key, string value, string comment) { }

        /// <summary>
        ///     called than IConfig removes a key
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="key">old key</param>
        /// <param name="value">old value</param>
        /// <param name="comment">old comment</param>
        protected virtual void OnKeyRemove(IConfig sender, string key, string value, string comment) { }

        /// <summary>
        ///     shows the IConfig info
        ///     format: [name] ;comment
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"[{_name}]" + (string.IsNullOrEmpty(_comment) ? string.Empty : $" ;{_comment}");
        }
    }
}