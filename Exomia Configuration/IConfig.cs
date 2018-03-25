using System;
using System.Collections.Generic;

namespace Exomia.Configuration
{
    /// <summary>
    ///     callback for IConfig events
    /// </summary>
    /// <param name="sender">IConfig</param>
    /// <param name="key">key</param>
    /// <param name="value">value</param>
    /// <param name="comment">comment</param>
    public delegate void ConfigKeyEventHandler(IConfig sender, string key, string value, string comment);

    /// <summary>
    ///     IConfig interface
    /// </summary>
    public interface IConfig
    {
        /// <summary>
        ///     return the IConfigSource parent
        /// </summary>
        IConfigSource ConfigSource { get; }

        /// <summary>
        ///     return the name of the config section
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     return the comment of the config section
        /// </summary>
        string Comment { get; set; }

        /// <summary>
        ///     get or set a value with the key in the config
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>value</returns>
        /// <exception cref="KeyNotFoundException">if the given key does not exists.</exception>
        string this[string key] { get; set; }

        /// <summary>
        ///     get all keys in the config
        /// </summary>
        IEnumerable<string> Keys { get; }

        /// <summary>
        ///     called than the IConfig set a key
        /// </summary>
        event ConfigKeyEventHandler KeySet;

        /// <summary>
        ///     called than the IConfig remove a key
        /// </summary>
        event ConfigKeyEventHandler KeyRemoved;

        /// <summary>
        ///     checks if the config contains the given key
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns><b>true</b> if successfully found; <b>false otherwise</b></returns>
        bool Contains(string key);

        /// <summary>
        ///     set a key - value pair in the config
        ///     if a key already exists it will be overwritten
        /// </summary>
        /// <typeparam name="T">struct</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <param name="comment">comment</param>
        void Set<T>(string key, T value, string comment = "") where T : IConvertible;

        /// <summary>
        ///     try to set a new key - value pair in the config
        ///     if a key allready exists nothing happen and false will be returned
        /// </summary>
        /// <typeparam name="T">struct</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <param name="comment">comment</param>
        /// <returns><b>true</b> if successfully added; <b>false otherwise</b></returns>
        bool TrySet<T>(string key, T value, string comment = "") where T : IConvertible;

        /// <summary>
        ///     set a key - value pair in the config
        ///     if a key already exists it will be overwritten
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="format">format</param>
        /// <param name="comment">comment</param>
        /// <param name="keys">keys</param>
        void SetExpanded(string key, string format, string comment, params string[] keys);

        /// <summary>
        ///     try to set a new key - value pair in the config
        ///     if a key allready exists nothing happen and false will be returned
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="format">format</param>
        /// <param name="comment">comment</param>
        /// <param name="keys">keys</param>
        bool TrySetExpanded(string key, string format, string comment, params string[] keys);

        /// <summary>
        ///     get a value from the given key in the config
        /// </summary>
        /// <typeparam name="T">struct</typeparam>
        /// <param name="key">key</param>
        /// <returns>value</returns>
        /// <exception cref="KeyNotFoundException">if the given key does not exists.</exception>
        T Get<T>(string key) where T : IConvertible;

        /// <summary>
        ///     get a value from the given key in the config
        /// </summary>
        /// <typeparam name="T">struct</typeparam>
        /// <param name="key">key</param>
        /// <returns>value</returns>
        /// <exception cref="KeyNotFoundException">if the given key does not exists.</exception>
        T GetExpanded<T>(string key) where T : IConvertible;

        /// <summary>
        ///     try to get a value from the given key in the config
        ///     if the key does not exists false will be returned
        /// </summary>
        /// <typeparam name="T">struct</typeparam>
        /// <param name="key">key</param>
        /// <param name="outValue">out value</param>
        /// <returns><b>true</b> if successfully; <b>false otherwise</b></returns>
        bool TryGet<T>(string key, out T outValue) where T : IConvertible;

        /// <summary>
        ///     try to get a value from the given key in the config
        ///     if the key does not exists false will be returned
        /// </summary>
        /// <typeparam name="T">struct</typeparam>
        /// <param name="key">key</param>
        /// <param name="outValue">out value</param>
        /// <returns><b>true</b> if successfully; <b>false otherwise</b></returns>
        bool TryGetExpanded<T>(string key, out T outValue) where T : IConvertible;

        /// <summary>
        ///     removes a given key from the config
        /// </summary>
        /// <param name="key">key</param>
        /// <returns><b>true</b> if successfully removed; <b>false otherwise</b></returns>
        bool Remove(string key);

        /// <summary>
        ///     trys to removes a given key from the config
        /// </summary>
        /// <param name="key">key</param>
        /// <returns><b>true</b> if successfully removed; <b>false otherwise</b></returns>
        bool TryRemove(string key);
    }

    internal struct ValueCommentPair
    {
        public string Value;
        public string Comment;

        public ValueCommentPair(string value, string comment)
        {
            Value = value;
            Comment = comment;
        }

        public override string ToString()
        {
            return (Value.Contains(";") ? $"\"{Value}\"" : $"{Value}") +
                   (string.IsNullOrEmpty(Comment) ? string.Empty : $" ;{Comment}");
        }
    }
}