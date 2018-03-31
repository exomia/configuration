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
    /// <inheritdoc />
    public sealed class XmlConfig : ConfigBase
    {
        #region Variables

        private readonly Dictionary<string, string[]> _keyInfos;
        private string[] _infos;

        #endregion

        #region Properties

        internal string[] Infos
        {
            get { return _infos; }
            set { _infos = value; }
        }

        internal Dictionary<string, string[]> KeyInfos
        {
            get { return _keyInfos; }
        }

        #endregion

        #region Constructors

        internal XmlConfig(IConfigSource configSource, string name, string comment = "")
            : base(configSource, name, comment)
        {
            _keyInfos = new Dictionary<string, string[]>();
        }

        #endregion

        #region Methods

        internal void AddKeyInternal(string key, string value, string comment, string[] infos = null)
        {
            Set(key, value, comment);
            if (infos != null)
            {
                _keyInfos.Add(key, infos);
            }
        }

        #endregion
    }
}