﻿#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Exomia.Configuration.Xml
{
    /// <summary>
    ///     An XML configuration source. This class cannot be inherited.
    /// </summary>
    public sealed class XmlConfigSource : ConfigSourceBase
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

        /// <summary>
        ///     Creates section node.
        /// </summary>
        /// <param name="doc">     The document. </param>
        /// <param name="section"> The section. </param>
        /// <param name="comment"> (Optional) The comment. </param>
        /// <returns>
        ///     The new section node.
        /// </returns>
        private static XmlNode CreateSectionNode(XmlDocument doc, string section, string comment = "")
        {
            XmlNode node = doc.CreateElement("section");

            XmlAttribute sectionAttr = doc.CreateAttribute("name");
            sectionAttr.Value = section;
            node.Attributes?.Append(sectionAttr);

            if (!string.IsNullOrEmpty(comment))
            {
                XmlAttribute commentAttr = doc.CreateAttribute("comment");
                commentAttr.Value = comment;
                node.Attributes?.Append(commentAttr);
            }

            return node;
        }

        /// <summary>
        ///     Creates kvc node.
        /// </summary>
        /// <param name="doc">     The document. </param>
        /// <param name="key">     The key. </param>
        /// <param name="value">   The value. </param>
        /// <param name="comment"> (Optional) The comment. </param>
        /// <returns>
        ///     The new kvc node.
        /// </returns>
        private static XmlNode CreateKvcNode(XmlDocument doc, string key, string value, string comment = "")
        {
            XmlNode node = doc.CreateElement("item");

            XmlAttribute keyAttr = doc.CreateAttribute("key");
            keyAttr.Value = key;

            // ReSharper disable once PossibleNullReferenceException
            node.Attributes.Append(keyAttr);

            XmlAttribute valueAttr = doc.CreateAttribute("value");
            valueAttr.Value = value;
            node.Attributes.Append(valueAttr);

            if (!string.IsNullOrEmpty(comment))
            {
                XmlAttribute commentAttr = doc.CreateAttribute("comment");
                commentAttr.Value = comment;
                node.Attributes.Append(commentAttr);
            }
            return node;
        }

        /// <inheritdoc />
        protected override IConfig CreateConfig(string section, string comment)
        {
            return new XmlConfig(this, section, comment);
        }

        /// <inheritdoc />
        protected override void OnReload()
        {
            if (string.IsNullOrEmpty(_saveFileName))
            {
                throw new FileNotFoundException("SaveFileName was not declared.", "SaveFileName");
            }
            _configs.Clear();
            XmlParser.Merge(new FileStream(_saveFileName, FileMode.Open, FileAccess.Read), this);
        }

        /// <inheritdoc />
        protected override void OnSave()
        {
            if (string.IsNullOrEmpty(_saveFileName))
            {
                throw new FileNotFoundException("SaveFileName was not declared.", "SaveFileName");
            }

            XmlDocument doc  = new XmlDocument();
            XmlNode     root = doc.CreateElement("config");
            doc.AppendChild(root);

            // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
            foreach (XmlConfig cfg in _configs.Values)
            {
                if (cfg.Infos != null)
                {
                    foreach (string info in cfg.Infos)
                    {
                        XmlComment commentNode = doc.CreateComment(info);
                        root.AppendChild(commentNode);
                    }
                }
                XmlNode section = CreateSectionNode(doc, cfg.Name, cfg.Comment);
                root.AppendChild(section);
                foreach (KeyValuePair<string, ValueCommentPair> item in cfg.VcPairs)
                {
                    ValueCommentPair pair = item.Value;
                    XmlNode          kvc  = CreateKvcNode(doc, item.Key, pair.Value, pair.Comment);
                    section.AppendChild(kvc);
                }
            }

            XmlWriterSettings settings = new XmlWriterSettings
            {
                Encoding           = Encoding.UTF8,
                ConformanceLevel   = ConformanceLevel.Document,
                OmitXmlDeclaration = false,
                CloseOutput        = true,
                Indent             = true,
                IndentChars        = "\t",
                NewLineHandling    = NewLineHandling.Replace
            };

            using (XmlWriter writer = XmlWriter.Create(_saveFileName, settings))
            {
                doc.WriteContentTo(writer);
            }
        }
    }
}