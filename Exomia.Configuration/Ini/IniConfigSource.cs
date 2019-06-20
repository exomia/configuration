#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

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

        /// <inheritdoc />
        protected override IConfig CreateConfig(string section, string comment)
        {
            return new IniConfig(this, section, comment);
        }

        /// <inheritdoc />
        protected override void OnReload()
        {
            if (string.IsNullOrEmpty(_saveFileName))
            {
                throw new FileNotFoundException("SaveFileName was not declared.", "SaveFileName");
            }
            _configs.Clear();
            IniParser.Merge(new FileStream(_saveFileName, FileMode.Open, FileAccess.Read), this);
        }

        /// <inheritdoc />
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