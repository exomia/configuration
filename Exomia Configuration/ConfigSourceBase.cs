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

using System.Collections.Generic;

namespace Exomia.Configuration
{
    /// <inheritdoc />
    public abstract class ConfigSourceBase : IConfigSource
    {
        #region Variables

        /// <summary>
        ///     Dictionary string, IConfig
        /// </summary>
        protected readonly Dictionary<string, IConfig> _configs;

        #endregion

        #region Properties

        /// <inheritdoc />
        public IConfig this[string section]
        {
            get { return Get(section); }
        }

        #endregion

        #region Constructors

        /// <inheritdoc />
        protected ConfigSourceBase()
        {
            _configs = new Dictionary<string, IConfig>();
        }

        #endregion

        #region Methods

        /// <inheritdoc />
        public event ConfigSourceEventHandler Reloaded;

        /// <inheritdoc />
        public event ConfigSourceEventHandler Saved;

        /// <inheritdoc />
        public IEnumerable<IConfig> GetConfigs()
        {
            foreach (IConfig cfg in _configs.Values)
            {
                yield return cfg;
            }
        }

        /// <inheritdoc />
        public IConfig Add(string section, string comment = "")
        {
            IConfig config = CreateConfig(section, comment);
            _configs.Add(section, config);
            return config;
        }

        /// <inheritdoc />
        public IConfig Get(string section)
        {
            return _configs[section];
        }

        /// <inheritdoc />
        public bool TryGet(string section, out IConfig config)
        {
            return _configs.TryGetValue(section, out config);
        }

        /// <inheritdoc />
        public void Merge(IConfigSource source)
        {
            foreach (IConfig config in source.GetConfigs())
            {
                if (_configs.ContainsKey(config.Name)) { continue; }

                IConfig cfg = Add(config.Name, config.Comment);

                foreach (string key in config.Keys)
                {
                    cfg.Set(key, config.Get<string>(key));
                }
            }
        }

        /// <inheritdoc />
        public void Reload()
        {
            OnReload();
            Reloaded?.Invoke(this);
        }

        /// <inheritdoc />
        public void Save()
        {
            OnSave();
            Saved?.Invoke(this);
        }

        /// <summary>
        ///     creates a new IConfig
        /// </summary>
        /// <param name="section">section</param>
        /// <param name="comment">comment</param>
        /// <returns></returns>
        protected abstract IConfig CreateConfig(string section, string comment);

        /// <summary>
        ///     called than IConfigSource reloaded
        /// </summary>
        protected virtual void OnReload() { }

        /// <summary>
        ///     called than IConfigSource saved
        /// </summary>
        protected virtual void OnSave() { }

        #endregion
    }
}