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

namespace Exomia.Configuration.Xml
{
    /// <summary>
    ///     An XML configuration. This class cannot be inherited.
    /// </summary>
    public sealed class XmlConfig : ConfigBase
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
        ///     Initializes a new instance of the <see cref="XmlConfig"/> class.
        /// </summary>
        /// <param name="configSource"> The configuration source. </param>
        /// <param name="name">         The name. </param>
        /// <param name="comment">      (Optional) The comment. </param>
        internal XmlConfig(IConfigSource configSource, string name, string comment = "")
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