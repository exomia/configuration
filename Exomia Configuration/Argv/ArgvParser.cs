using System;
using System.Text.RegularExpressions;

namespace Exomia.Configuration.Argv
{
    //TODO: better parsing from command line support -p="" --p="" -p=44 --p=45
    /// <summary>
    ///     ArgvParser class
    /// </summary>
    public static class ArgvParser
    {
        internal const string ESCAPE_COMMENT = ";";

        private static readonly Regex s_r1;

        static ArgvParser()
        {
            s_r1 = new Regex(
                $"^(.+?)\\s*=\\s*(?([\"|\'])[\"|\'](.+)[\"|\'](?:\\s*{ESCAPE_COMMENT}\\s*(.+))?|([^;\\n]+)?(?:\\s*{ESCAPE_COMMENT}\\s*(.+))?)",
                RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);
        }

        private static bool GetKeyValueCommentFromArgv(string line, out string key, out string value,
            out string comment)
        {
            key = string.Empty;
            value = string.Empty;
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

        /// <summary>
        ///     parse a command line to a ArgvConfigSource
        /// </summary>
        /// <param name="argv">command line args</param>
        /// <param name="startIndex">start index</param>
        /// <param name="section">section</param>
        /// <param name="comment">comment</param>
        /// <returns>ArgvConfigSource</returns>
        public static ArgvConfigSource Parse(string[] argv, int startIndex, string section, string comment = "")
        {
            ArgvConfigSource source = new ArgvConfigSource();
            Merge(source, argv, startIndex, section, comment);
            return source;
        }

        /// <summary>
        ///     merge a command line with an existing ArgvConfigSource
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="argv">command line args</param>
        /// <param name="startIndex">start index</param>
        /// <param name="section">section</param>
        /// <param name="comment">comment</param>
        public static void Merge(ArgvConfigSource source, string[] argv, int startIndex, string section,
            string comment = "")
        {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (argv == null) { throw new ArgumentNullException(nameof(argv)); }

            ArgvConfig config = (ArgvConfig)source.Add(section, comment);

            for (int i = startIndex; i < argv.Length; i++)
            {
                string buffer = argv[i].Trim('\r', '\n', ' ');
                if (string.IsNullOrEmpty(buffer)) { continue; }

                if (GetKeyValueCommentFromArgv(buffer, out string key, out string value, out comment))
                {
                    config.VCPairs.Add(key, new ValueCommentPair(value, comment));
                }
                else
                {
                    config.VCPairs.Add(i.ToString(), new ValueCommentPair(buffer, string.Empty));
                }
            }
        }
    }
}