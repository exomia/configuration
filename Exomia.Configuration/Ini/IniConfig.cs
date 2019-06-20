#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Collections.Generic;

namespace Exomia.Configuration.Ini
{
    /// <summary>
    ///     An initialise configuration. This class cannot be inherited.
    /// </summary>
    public sealed class IniConfig : ConfigBase
    {
        /// <summary>
        ///     The key infos.
        /// </summary>
        private readonly Dictionary<string, string[]> _keyInfos;

        /// <summary>
        ///     The infos.
        /// </summary>
        private string[] _infos;

        /// <summary>
        ///     Gets or sets the infos.
        /// </summary>
        /// <value>
        ///     The infos.
        /// </value>
        internal string[] Infos
        {
            get { return _infos; }
            set { _infos = value; }
        }

        /// <summary>
        ///     Gets the key infos.
        /// </summary>
        /// <value>
        ///     The key infos.
        /// </value>
        internal Dictionary<string, string[]> KeyInfos
        {
            get { return _keyInfos; }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="IniConfig" /> class.
        /// </summary>
        /// <param name="configSource"> The configuration source. </param>
        /// <param name="name">         The name. </param>
        /// <param name="comment">      (Optional) The comment. </param>
        internal IniConfig(IConfigSource configSource, string name, string comment = "")
            : base(configSource, name, comment)
        {
            _keyInfos = new Dictionary<string, string[]>();
        }

        /// <summary>
        ///     Adds a key internal.
        /// </summary>
        /// <param name="key">     The key. </param>
        /// <param name="value">   The value. </param>
        /// <param name="comment"> The comment. </param>
        /// <param name="infos">   (Optional) The infos. </param>
        internal void AddKeyInternal(string key, string value, string comment, string[] infos = null)
        {
            Set(key, value, comment);
            if (infos != null)
            {
                _keyInfos.Add(key, infos);
            }
        }
    }
}