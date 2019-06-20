#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Collections.Generic;

namespace Exomia.Configuration
{
    /// <summary>
    ///     A configuration source base.
    /// </summary>
    public abstract class ConfigSourceBase : IConfigSource
    {
        /// <summary>
        ///     Occurs when Reloaded.
        /// </summary>
        public event ConfigSourceEventHandler Reloaded;

        /// <summary>
        ///     Occurs when Saved.
        /// </summary>
        public event ConfigSourceEventHandler Saved;

        /// <summary>
        ///     The configs.
        /// </summary>
        protected readonly Dictionary<string, IConfig> _configs;

        /// <inheritdoc />
        public IConfig this[string section]
        {
            get { return Get(section); }
        }

        /// <inheritdoc />
        protected ConfigSourceBase()
        {
            _configs = new Dictionary<string, IConfig>();
        }

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
        ///     creates a new IConfig.
        /// </summary>
        /// <param name="section"> section. </param>
        /// <param name="comment"> comment. </param>
        /// <returns>
        ///     The new configuration.
        /// </returns>
        protected abstract IConfig CreateConfig(string section, string comment);

        /// <summary>
        ///     called than IConfigSource reloaded.
        /// </summary>
        protected virtual void OnReload() { }

        /// <summary>
        ///     called than IConfigSource saved.
        /// </summary>
        protected virtual void OnSave() { }
    }
}