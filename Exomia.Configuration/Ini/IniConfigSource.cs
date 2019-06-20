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
using System.IO;

namespace Exomia.Configuration.Ini
{
    /// <summary>
    ///     An initialise configuration source. This class cannot be inherited.
    /// </summary>
    public sealed class IniConfigSource : ConfigSourceBase
    {
        /// <summary>
        ///     Filename of the save file.
        /// </summary>
        private string _saveFileName = string.Empty;


        /// <summary>
        ///     Gets or sets the filename of the save file.
        /// </summary>
        /// <value>
        ///     The filename of the save file.
        /// </value>
        public string SaveFileName
        {
            get { return _saveFileName; }
            set { _saveFileName = value; }
        }

        /// <inheritdoc/>
        protected override IConfig CreateConfig(string section, string comment)
        {
            return new IniConfig(this, section, comment);
        }

        /// <inheritdoc/>
        protected override void OnReload()
        {
            if (string.IsNullOrEmpty(_saveFileName))
            {
                throw new FileNotFoundException("SaveFileName was not declared.", "SaveFileName");
            }
            _configs.Clear();
            IniParser.Merge(new FileStream(_saveFileName, FileMode.Open, FileAccess.Read), this);
        }

        /// <inheritdoc/>
        protected override void OnSave()
        {
            if (string.IsNullOrEmpty(_saveFileName))
            {
                throw new FileNotFoundException("SaveFileName was not declared.", "SaveFileName");
            }
            using (StreamWriter sw =
                new StreamWriter(new FileStream(_saveFileName, FileMode.OpenOrCreate, FileAccess.Write)))
            {
                // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
                foreach (IniConfig cfg in _configs.Values)
                {
                    if (cfg.Infos != null)
                    {
                        foreach (string info in cfg.Infos)
                        {
                            sw.WriteLine(info);
                        }
                    }
                    sw.WriteLine(
                        $"[{cfg.Name}]{(string.IsNullOrEmpty(cfg.Comment) ? "" : $" {IniParser.ESCAPE_COMMENT}{cfg.Comment}")}");
                    foreach (KeyValuePair<string, ValueCommentPair> item in cfg.VcPairs)
                    {
                        ValueCommentPair pair = item.Value;

                        if (cfg.KeyInfos.TryGetValue(item.Key, out string[] infos))
                        {
                            foreach (string info in infos)
                            {
                                sw.WriteLine(info);
                            }
                        }
                        sw.WriteLine(
                            $"{item.Key} = {(pair.Value.Contains(";") ? $"\"{pair.Value}\"" : $"{pair.Value}")}{(string.IsNullOrEmpty(pair.Comment) ? "" : $" ;{pair.Comment}")}");
                    }
                    sw.WriteLine();
                }
            }
        }
    }
}