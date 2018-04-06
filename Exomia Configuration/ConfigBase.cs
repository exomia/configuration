#region MIT License

// Copyright (c) 2018 exomia - Daniel Bätz
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Exomia.Configuration
{
    /// <inheritdoc />
    public abstract class ConfigBase : IConfig
    {
        #region Variables

        private static readonly Regex s_r1;
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

        #endregion

        #region Properties

        internal Dictionary<string, ValueCommentPair> VcPairs
        {
            get { return _vcPairs; }
        }

        /// <inheritdoc />
        public IConfigSource ConfigSource
        {
            get { return _configSource; }
        }

        /// <inheritdoc />
        public string Name
        {
            get { return _name; }
        }

        /// <inheritdoc />
        public string Comment
        {
            get { return _comment; }
            set { _comment = value; }
        }

        /// <inheritdoc />
        public IEnumerable<string> Keys
        {
            get { return _vcPairs.Keys; }
        }

        /// <inheritdoc />
        public virtual string this[string key]
        {
            get { return _vcPairs[key].Value; }
            set { Set(key, value); }
        }

        #endregion

        #region Constructors

        static ConfigBase()
        {
            s_r1 = new Regex(@"\${(.+?)(?:\.(.+?))?\}");
        }

        /// <summary>
        ///     ConfigBase constructor
        /// </summary>
        protected ConfigBase(IConfigSource configSource, string name, string comment = "")
        {
            _name = name;
            _configSource = configSource;
            _comment = comment;

            _vcPairs = new Dictionary<string, ValueCommentPair>();
        }

        #endregion

        #region Methods

        /// <inheritdoc />
        public event ConfigKeyEventHandler KeySet;

        /// <inheritdoc />
        public event ConfigKeyEventHandler KeyRemoved;

        /// <inheritdoc />
        public bool Contains(string key)
        {
            return _vcPairs.ContainsKey(key);
        }

        /// <inheritdoc />
        public virtual void Set<T>(string key, T value, string comment = "") where T : IConvertible
        {
            ValueCommentPair buffer = new ValueCommentPair(value.ToString(CultureInfo.InvariantCulture), comment);
            _vcPairs[key] = buffer;

            OnKeySet(this, key, buffer.Value, buffer.Comment);
            KeySet?.Invoke(this, key, buffer.Value, buffer.Comment);
        }

        /// <inheritdoc />
        public virtual bool TrySet<T>(string key, T value, string comment = "") where T : IConvertible
        {
            if (_vcPairs.ContainsKey(key)) { return false; }
            Set(key, value, comment);
            return true;
        }

        /// <inheritdoc />
        public virtual void SetExpanded(string key, string format, string comment, params string[] keys)
        {
            if (comment == null)
            {
                comment = string.Empty;
            }
            Set(key, string.Format(format, keys.Select(x => (object)$"${{{x}}}").ToArray()), comment);
        }

        /// <inheritdoc />
        public virtual bool TrySetExpanded(string key, string format, string comment, params string[] keys)
        {
            if (comment == null)
            {
                comment = string.Empty;
            }

            return TrySet(key, string.Format(format, keys.Select(x => (object)$"${{{x}}}").ToArray()), comment);
        }

        /// <inheritdoc />
        public virtual T Get<T>(string key) where T : IConvertible
        {
            Type type = typeof(T);
            if (type != typeof(string) && !type.IsPrimitive) { return default(T); }

            return (T)Convert.ChangeType(_vcPairs[key].Value, type);
        }

        /// <inheritdoc />
        public virtual T GetExpanded<T>(string key) where T : IConvertible
        {
            Type type = typeof(T);
            if (type != typeof(string) && !type.IsPrimitive) { return default(T); }

            return (T)Convert.ChangeType(ExpandValue(_vcPairs[key].Value), type);
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public virtual bool TryRemove(string key)
        {
            return !_vcPairs.ContainsKey(key) && Remove(key);
        }

        /// <summary>
        ///     shows the IConfig info
        ///     format: [name] ;comment
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"[{_name}]" + (string.IsNullOrEmpty(_comment) ? string.Empty : $" ;{_comment}");
        }

        /// <summary>
        ///     ExpandValue
        /// </summary>
        protected string ExpandValue(string value)
        {
            Match match;
            while ((match = s_r1.Match(value)).Success)
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

        #endregion
    }
}