#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Exomia.Configuration.Ini
{
    /// <summary>
    ///     An init file parser.
    /// </summary>
    public static class IniParser
    {
        /// <summary>
        ///     The escape comment.
        /// </summary>
        internal const string ESCAPE_COMMENT = ";";

        /// <summary>
        ///     The first s r.
        /// </summary>
        private static readonly Regex s_r1;

        /// <summary>
        ///     The second s r.
        /// </summary>
        private static readonly Regex s_r2;

        /// <summary>
        ///     Initializes static members of the <see cref="IniParser" /> class.
        /// </summary>
        static IniParser()
        {
            s_r1 = new Regex(
                $"^(.+?)\\s*=\\s*(?([\"|\'])[\"|\'](.+)[\"|\'](?:\\s*{ESCAPE_COMMENT}\\s*(.+))?|([^;\\n]+)?(?:\\s*{ESCAPE_COMMENT}\\s*(.+))?)",
                RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);
            s_r2 = new Regex($"^\\[(.+)\\]\\s*(?:{ESCAPE_COMMENT}(.+))?");
        }

        /// <summary>
        ///     parse a ini file to a IniConfigSource.
        /// </summary>
        /// <param name="fileName"> fileName. </param>
        /// <returns>
        ///     IniConfigSource.
        /// </returns>
        public static IniConfigSource Parse(string fileName)
        {
            return Parse(new FileStream(fileName, FileMode.Open, FileAccess.Read), null, fileName);
        }

        /// <summary>
        ///     parse a ini file to a IniConfigSource.
        /// </summary>
        /// <param name="stream">   stream. </param>
        /// <param name="fileName"> (Optional) fileName. </param>
        /// <returns>
        ///     IniConfigSource.
        /// </returns>
        public static IniConfigSource Parse(Stream stream, string fileName = "")
        {
            return Parse(stream, null, fileName);
        }

        /// <summary>
        ///     merge a ini file with an existing IniConfigSource.
        /// </summary>
        /// <param name="fileName"> fileName. </param>
        /// <param name="source">   source. </param>
        public static void Merge(string fileName, IniConfigSource source)
        {
            Merge(new FileStream(fileName, FileMode.Open, FileAccess.Read), source);
        }

        /// <summary>
        ///     merge a ini file stream with an existing IniConfigSource.
        /// </summary>
        /// <param name="stream"> stream. </param>
        /// <param name="source"> source. </param>
        /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
        public static void Merge(Stream stream, IniConfigSource source)
        {
            if (stream        == null) { throw new ArgumentNullException(nameof(stream)); }
            if (source        == null) { throw new ArgumentNullException(nameof(source)); }
            if (stream.Length <= 0) { return; }

            stream.Position = 0;
            using (StreamReader sr = new StreamReader(stream))
            {
                IniConfig    config = null;
                List<string> infos  = new List<string>();
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine()?.Trim('\r', '\n', ' ');
                    if (string.IsNullOrEmpty(line)) { continue; }
                    if (line.StartsWith(";"))
                    {
                        infos.Add(line);
                        continue;
                    }
                    string comment;
                    if (line.StartsWith("["))
                    {
                        if (GetSection(line, out string section, out comment))
                        {
                            config       = (IniConfig)source.Add(section, comment);
                            config.Infos = infos.ToArray();
                            infos.Clear();
                        }
                        continue;
                    }

                    if (config != null && GetKeyValueCommentFromLine(
                            line, out string key, out string value, out comment))
                    {
                        config.AddKeyInternal(key, value, comment, infos.ToArray());
                        infos.Clear();
                    }
                }
            }
        }

        /// <summary>
        ///     parse a ini file to a IniConfigSource.
        /// </summary>
        /// <param name="stream">   stream. </param>
        /// <param name="source">   (Optional) source. </param>
        /// <param name="fileName"> (Optional) fileName. </param>
        /// <returns>
        ///     IniConfigSource.
        /// </returns>
        internal static IniConfigSource Parse(Stream stream, IniConfigSource source = null, string fileName = "")
        {
            if (source == null)
            {
                source = new IniConfigSource { SaveFileName = fileName };
            }
            Merge(stream, source);
            return source;
        }

        /// <summary>
        ///     Gets a section.
        /// </summary>
        /// <param name="line">    The line. </param>
        /// <param name="section"> [out] The section. </param>
        /// <param name="comment"> [out] The comment. </param>
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        /// <exception cref="Exception"> Thrown when an exception error condition occurs. </exception>
        private static bool GetSection(string line, out string section, out string comment)
        {
            Match match = s_r2.Match(line);
            if (!match.Success)
            {
                throw new Exception("the section is not valid");
            }

            section = match.Groups[1].ToString().Trim('\r', '\n', ' ');
            comment = match.Groups[2].ToString().Trim('\r', '\n', ' ');
            return true;
        }

        /// <summary>
        ///     Gets key value comment from line.
        /// </summary>
        /// <param name="line">    The line. </param>
        /// <param name="key">     [out] The key. </param>
        /// <param name="value">   [out] The value. </param>
        /// <param name="comment"> [out] The comment. </param>
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        private static bool GetKeyValueCommentFromLine(string     line, out string key, out string value,
                                                       out string comment)
        {
            key     = string.Empty;
            value   = string.Empty;
            comment = string.Empty;

            Match match = s_r1.Match(line);
            if (!match.Success)
            {
                return false;
            }

            key = match.Groups[1].ToString().Trim('\r', '\n', ' ');
            if (match.Groups[2].Success)
            {
                value = match.Groups[2].ToString().Trim('\r', '\n', ' ');
                if (match.Groups[3].Success)
                {
                    comment = match.Groups[3].ToString().Trim('\r', '\n', ' ');
                }
                return true;
            }

            if (match.Groups[4].Success)
            {
                value = match.Groups[4].ToString().Trim('\r', '\n', ' ');
                if (match.Groups[5].Success)
                {
                    comment = match.Groups[5].ToString().Trim('\r', '\n', ' ');
                }
                return true;
            }

            return false;
        }
    }
}