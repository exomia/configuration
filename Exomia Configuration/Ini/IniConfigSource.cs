using System.Collections.Generic;
using System.IO;

namespace Exomia.Configuration.Ini
{
    /// <inheritdoc />
    public sealed class IniConfigSource : ConfigSourceBase
    {
        private string _saveFileName = string.Empty;

        /// <summary>
        ///     SaveFileName
        /// </summary>
        public string SaveFileName
        {
            get { return _saveFileName; }
            set { _saveFileName = value; }
        }

        /// <inheritdoc />
        protected override IConfig CreateConfig(string section, string comment = "")
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
                        $"[{cfg.Name}]" + (string.IsNullOrEmpty(cfg.Comment)
                            ? ""
                            : $" {IniParser.ESCAPE_COMMENT}{cfg.Comment}"));
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
                            $"{item.Key} = " + (pair.Value.Contains(";") ? $"\"{pair.Value}\"" : $"{pair.Value}") +
                            (string.IsNullOrEmpty(pair.Comment) ? "" : $" ;{pair.Comment}"));
                    }
                    sw.WriteLine();
                }
            }
        }
    }
}