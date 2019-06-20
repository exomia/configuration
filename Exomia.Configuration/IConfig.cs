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

using System;
using System.Collections.Generic;

namespace Exomia.Configuration
{
    /// <summary>
    ///     callback for IConfig events.
    /// </summary>
    /// <param name="sender">  The sender. </param>
    /// <param name="key">     The key. </param>
    /// <param name="value">   The value. </param>
    /// <param name="comment"> The comment. </param>
    public delegate void ConfigKeyEventHandler(IConfig sender, string key, string value, string comment);

    /// <summary>
    ///     IConfig interface.
    /// </summary>
    public interface IConfig
    {
        /// <summary>
        ///     called than the IConfig set a key.
        /// </summary>
        event ConfigKeyEventHandler KeySet;

        /// <summary>
        ///     called than the IConfig remove a key.
        /// </summary>
        event ConfigKeyEventHandler KeyRemoved;

        /// <summary>
        ///     return the IConfigSource parent.
        /// </summary>
        /// <value>
        ///     The configuration source.
        /// </value>
        IConfigSource ConfigSource { get; }

        /// <summary>
        ///     return the name of the config section.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        string Name { get; }

        /// <summary>
        ///     return the comment of the config section.
        /// </summary>
        /// <value>
        ///     The comment.
        /// </value>
        string Comment { get; set; }

        /// <summary>
        ///     get or set a value with the key in the config.
        /// </summary>
        /// <param name="key"> key. </param>
        /// <returns>
        ///     value.
        /// </returns>
        ///
        /// <exception cref="KeyNotFoundException"> if the given key does not exists. </exception>
        string this[string key] { get; set; }

        /// <summary>
        ///     get all keys in the config.
        /// </summary>
        /// <value>
        ///     The keys.
        /// </value>
        IEnumerable<string> Keys { get; }

        /// <summary>
        ///     checks if the config contains the given key.
        /// </summary>
        /// <param name="key"> Key. </param>
        /// <returns>
        ///     <b>true</b> if successfully found; <b>false otherwise</b>
        /// </returns>
        bool Contains(string key);

        /// <summary>
        ///     set a key - value pair in the config if a key already exists it will be overwritten.
        /// </summary>
        /// <typeparam name="T"> struct. </typeparam>
        /// <param name="key">     key. </param>
        /// <param name="value">   value. </param>
        /// <param name="comment"> (Optional) comment. </param>
        void Set<T>(string key, T value, string comment = "") where T : IConvertible;

        /// <summary>
        ///     try to set a new key - value pair in the config if a key all ready exists nothing happen
        ///     and false will be returned.
        /// </summary>
        /// <typeparam name="T"> struct. </typeparam>
        /// <param name="key">     key. </param>
        /// <param name="value">   value. </param>
        /// <param name="comment"> (Optional) comment. </param>
        /// <returns>
        ///     <b>true</b> if successfully added; <b>false otherwise</b>
        /// </returns>
        bool TrySet<T>(string key, T value, string comment = "") where T : IConvertible;

        /// <summary>
        ///     set a key - value pair in the config if a key already exists it will be overwritten.
        /// </summary>
        /// <param name="key">     key. </param>
        /// <param name="format">  format. </param>
        /// <param name="comment"> comment. </param>
        /// <param name="keys">    keys. </param>
        void SetExpanded(string key, string format, string comment, params string[] keys);

        /// <summary>
        ///     try to set a new key - value pair in the config if a key all ready exists nothing happen
        ///     and false will be returned.
        /// </summary>
        /// <param name="key">     key. </param>
        /// <param name="format">  format. </param>
        /// <param name="comment"> comment. </param>
        /// <param name="keys">    keys. </param>
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        bool TrySetExpanded(string key, string format, string comment, params string[] keys);

        /// <summary>
        ///     get a value from the given key in the config.
        /// </summary>
        /// <typeparam name="T"> struct. </typeparam>
        /// <param name="key"> key. </param>
        /// <returns>
        ///     value.
        /// </returns>
        /// <exception cref="KeyNotFoundException"> if the given key does not exists. </exception>
        T Get<T>(string key) where T : IConvertible;

        /// <summary>
        ///     get a value from the given key in the config.
        /// </summary>
        /// <typeparam name="T"> struct. </typeparam>
        /// <param name="key"> key. </param>
        /// <returns>
        ///     value.
        /// </returns>
        /// <exception cref="KeyNotFoundException"> if the given key does not exists. </exception>
        T GetExpanded<T>(string key) where T : IConvertible;

        /// <summary>
        ///     try to get a value from the given key in the config if the key does not exists false will
        ///     be returned.
        /// </summary>
        /// <typeparam name="T"> struct. </typeparam>
        /// <param name="key">      key. </param>
        /// <param name="outValue"> [out] out value. </param>
        /// <returns>
        ///     <b>true</b> if successfully; <b>false otherwise</b>
        /// </returns>
        bool TryGet<T>(string key, out T outValue) where T : IConvertible;

        /// <summary>
        ///     try to get a value from the given key in the config if the key does not exists false will
        ///     be returned.
        /// </summary>
        /// <typeparam name="T"> struct. </typeparam>
        /// <param name="key">      key. </param>
        /// <param name="outValue"> [out] out value. </param>
        /// <returns>
        ///     <b>true</b> if successfully; <b>false otherwise</b>
        /// </returns>
        bool TryGetExpanded<T>(string key, out T outValue) where T : IConvertible;

        /// <summary>
        ///     removes a given key from the config.
        /// </summary>
        /// <param name="key"> key. </param>
        /// <returns>
        ///     <b>true</b> if successfully removed; <b>false otherwise</b>
        /// </returns>
        bool Remove(string key);

        /// <summary>
        ///     tries to removes a given key from the config.
        /// </summary>
        /// <param name="key"> key. </param>
        /// <returns>
        ///     <b>true</b> if successfully removed; <b>false otherwise</b>
        /// </returns>
        bool TryRemove(string key);
    }

    /// <summary>
    ///     A value comment pair.
    /// </summary>
    struct ValueCommentPair
    {
        /// <summary>
        ///     The value.
        /// </summary>
        public string Value;
        /// <summary>
        ///     The comment.
        /// </summary>
        public string Comment;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ValueCommentPair"/> struct.
        /// </summary>
        /// <param name="value">   The value. </param>
        /// <param name="comment"> The comment. </param>
        public ValueCommentPair(string value, string comment)
        {
            Value = value;
            Comment = comment;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return (Value.Contains(";") ? $"\"{Value}\"" : $"{Value}") +
                   (string.IsNullOrEmpty(Comment) ? string.Empty : $" ;{Comment}");
        }
    }
}