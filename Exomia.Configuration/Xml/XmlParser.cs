﻿#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Exomia.Configuration.Xml
{
    /// <summary>
    ///     An XML parser.
    /// </summary>
    public static class XmlParser
    {
        /// <summary>
        ///     parse a xml file to a XmlConfigSource.
        /// </summary>
        /// <param name="fileName"> fileName. </param>
        /// <returns>
        ///     XmlConfigSource.
        /// </returns>
        public static XmlConfigSource Parse(string fileName)
        {
            return Parse(new FileStream(fileName, FileMode.Open, FileAccess.Read), null, fileName);
        }

        /// <summary>
        ///     parse a xml file to a IniConfigSource.
        /// </summary>
        /// <param name="stream">   stream. </param>
        /// <param name="fileName"> (Optional) fileName. </param>
        /// <returns>
        ///     XmlConfigSource.
        /// </returns>
        public static XmlConfigSource Parse(Stream stream, string fileName = "")
        {
            return Parse(stream, null, fileName);
        }

        /// <summary>
        ///     merge a xml file with an existing XmlConfigSource.
        /// </summary>
        /// <param name="fileName"> fileName. </param>
        /// <param name="source">   source. </param>
        public static void Merge(string fileName, XmlConfigSource source)
        {
            Merge(new FileStream(fileName, FileMode.Open, FileAccess.Read), source);
        }

        /// <summary>
        ///     merge a xml file stream with an existing XmlConfigSource.
        /// </summary>
        /// <param name="stream"> stream. </param>
        /// <param name="source"> source. </param>
        /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
        /// <exception cref="Exception">             Thrown when an exception error condition occurs. </exception>
        public static void Merge(Stream stream, XmlConfigSource source)
        {
            if (stream        == null) { throw new ArgumentNullException(nameof(stream)); }
            if (source        == null) { throw new ArgumentNullException(nameof(source)); }
            if (stream.Length <= 0) { return; }

            XmlConfig    config = null;
            List<string> infos  = new List<string>();

            stream.Position = 0;
            XmlReaderSettings settings = new XmlReaderSettings
            {
                CloseInput = true, ConformanceLevel = ConformanceLevel.Document, IgnoreComments = false
            };

            using (XmlReader reader = XmlReader.Create(stream, settings))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            string comment;
                            switch (reader.Name.ToLower())
                            {
                                case "config":
                                    infos.Clear();
                                    break;
                                case "section":
                                    if (reader.HasAttributes)
                                    {
                                        if (!reader.MoveToAttribute("name"))
                                        {
                                            throw new Exception("invalid section no name attribute found!");
                                        }
                                        string section = reader.Value;

                                        comment = string.Empty;
                                        if (reader.MoveToAttribute("comment"))
                                        {
                                            comment = reader.Value;
                                        }
                                        config       = (XmlConfig)source.Add(section, comment);
                                        config.Infos = infos.ToArray();
                                        infos.Clear();
                                    }
                                    break;
                                case "item":
                                    if (config != null)
                                    {
                                        if (reader.HasAttributes)
                                        {
                                            if (!reader.MoveToAttribute("key"))
                                            {
                                                throw new Exception("invalid item no key attribute found!");
                                            }
                                            string key = reader.Value;

                                            if (!reader.MoveToAttribute("value"))
                                            {
                                                throw new Exception("invalid item no value attribute found!");
                                            }
                                            string value = reader.Value;

                                            comment = string.Empty;
                                            if (reader.MoveToAttribute("comment"))
                                            {
                                                comment = reader.Value;
                                            }

                                            config.AddKeyInternal(key, value, comment, infos.ToArray());
                                            infos.Clear();
                                        }
                                    }
                                    break;
                            }
                            break;
                        case XmlNodeType.Comment:
                            infos.Add(reader.Value);
                            break;
                    }
                }
            }
        }

        /// <summary>
        ///     parse a xml file to a IniConfigSource.
        /// </summary>
        /// <param name="stream">   stream. </param>
        /// <param name="source">   source. </param>
        /// <param name="fileName"> (Optional) fileName. </param>
        /// <returns>
        ///     XmlConfigSource.
        /// </returns>
        internal static XmlConfigSource Parse(Stream stream, XmlConfigSource source, string fileName = "")
        {
            if (source == null)
            {
                source = new XmlConfigSource { SaveFileName = fileName };
            }
            Merge(stream, source);
            return source;
        }
    }
}