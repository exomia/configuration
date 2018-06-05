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
    /// <summary>
    ///     callback for IConfigSource events
    /// </summary>
    /// <param name="sender">IConfigSource</param>
    public delegate void ConfigSourceEventHandler(IConfigSource sender);

    /// <summary>
    ///     IConfigSource interface
    /// </summary>
    public interface IConfigSource
    {
        #region Variables

        /// <summary>
        ///     called than the IConfigSource is reloaded
        /// </summary>
        event ConfigSourceEventHandler Reloaded;

        /// <summary>
        ///     called than the IConfigSource is saved
        /// </summary>
        event ConfigSourceEventHandler Saved;

        #endregion

        #region Properties

        /// <summary>
        ///     returns a IConfig from a given section
        /// </summary>
        /// <param name="section">section</param>
        /// <returns>IConfig</returns>
        /// <exception cref="KeyNotFoundException">if the given section does not exists.</exception>
        IConfig this[string section] { get; }

        #endregion

        #region Methods

        /// <summary>
        ///     gets all configs in the config source
        /// </summary>
        /// <returns>IEnumerable IConfig</returns>
        IEnumerable<IConfig> GetConfigs();

        /// <summary>
        ///     adds a new config to the config source
        /// </summary>
        /// <param name="section">section</param>
        /// <param name="comment">comment</param>
        /// <returns>new config</returns>
        IConfig Add(string section, string comment = "");

        /// <summary>
        ///     returns a IConfig from a given section
        /// </summary>
        /// <param name="section">section</param>
        /// <returns>IConfig</returns>
        /// <exception cref="KeyNotFoundException">if the given section does not exists.</exception>
        IConfig Get(string section);

        /// <summary>
        ///     try to get a config from the given section in the config source
        ///     if the section does not exists false will be returned
        /// </summary>
        /// <param name="section">section</param>
        /// <param name="config">out config</param>
        /// <returns><b>true</b> if successfully; <b>false otherwise</b></returns>
        bool TryGet(string section, out IConfig config);

        /// <summary>
        ///     merge all configs from the given cofig source into current one
        /// </summary>
        /// <param name="source">Config source</param>
        void Merge(IConfigSource source);

        /// <summary>
        ///     reloads the config source
        ///     Merged config sources will be lost
        /// </summary>
        void Reload();

        /// <summary>
        ///     saves the current config source
        /// </summary>
        void Save();

        #endregion
    }
}