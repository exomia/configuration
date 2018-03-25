﻿using System.Collections.Generic;

namespace Exomia.Configuration.Xml
{
    /// <summary>
    ///     implements <see cref="ConfigBase" /> base class.
    /// </summary>
    public sealed class XmlConfig : ConfigBase
    {
        private readonly Dictionary<string, string[]> _keyInfos;
        private string[] _infos;

        internal XmlConfig(IConfigSource configSource, string name, string comment = "")
            : base(configSource, name, comment)
        {
            _keyInfos = new Dictionary<string, string[]>();
        }

        internal string[] Infos
        {
            get { return _infos; }
            set { _infos = value; }
        }

        internal Dictionary<string, string[]> KeyInfos
        {
            get { return _keyInfos; }
        }

        internal void AddKeyInternal(string key, string value, string comment, string[] infos = null)
        {
            VCPairs.Add(key, new ValueCommentPair(value, comment));
            if (infos != null)
            {
                _keyInfos.Add(key, infos);
            }
        }
    }
}