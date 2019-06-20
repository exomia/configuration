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
    ///     callback for IConfigSource events.
    /// </summary>
    /// <param name="sender"> The sender. </param>
    public delegate void ConfigSourceEventHandler(IConfigSource sender);

    /// <summary>
    ///     Interface for configuration source.
    /// </summary>
    public interface IConfigSource
    {
        /// <summary>
        ///     called than the IConfigSource is reloaded.
        /// </summary>
        event ConfigSourceEventHandler Reloaded;

        /// <summary>
        ///     called than the IConfigSource is saved.
        /// </summary>
        event ConfigSourceEventHandler Saved;

        /// <summary>
        ///     returns a IConfig from a given section.
        /// </summary>
        /// <param name="section"> section. </param>
        /// <returns>
        ///     IConfig.
        /// </returns>
        /// <exception cref="KeyNotFoundException"> if the given section does not exists. </exception>
        IConfig this[string section] { get; }

        /// <summary>
        ///     gets all configs in the config source.
        /// </summary>
        /// <returns>
        ///     IEnumerable IConfig.
        /// </returns>
        IEnumerable<IConfig> GetConfigs();

        /// <summary>
        ///     adds a new config to the config source.
        /// </summary>
        /// <param name="section"> section. </param>
        /// <param name="comment"> (Optional) comment. </param>
        /// <returns>
        ///     new config.
        /// </returns>
        IConfig Add(string section, string comment = "");

        /// <summary>
        ///     returns a IConfig from a given section.
        /// </summary>
        /// <param name="section"> section. </param>
        /// <returns>
        ///     IConfig.
        /// </returns>
        /// <exception cref="KeyNotFoundException"> if the given section does not exists. </exception>
        IConfig Get(string section);

        /// <summary>
        ///     try to get a config from the given section in the config source if the section does not
        ///     exists false will be returned.
        /// </summary>
        /// <param name="section"> section. </param>
        /// <param name="config">  [out] out config. </param>
        /// <returns>
        ///     <b>true</b> if successfully; <b>false otherwise</b>
        /// </returns>
        bool TryGet(string section, out IConfig config);

        /// <summary>
        ///     merge all configs from the given config source into current one.
        /// </summary>
        /// <param name="source"> Config source. </param>
        void Merge(IConfigSource source);

        /// <summary>
        ///     reloads the config source Merged config sources will be lost.
        /// </summary>
        void Reload();

        /// <summary>
        ///     saves the current config source.
        /// </summary>
        void Save();
    }
}