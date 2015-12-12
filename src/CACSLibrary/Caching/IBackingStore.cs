using System;
using System.Collections;

namespace CACSLibrary.Caching
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBackingStore : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newCacheItem"></param>
        void Add(CacheItem newCacheItem);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        void Remove(string key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="timestamp"></param>
        void UpdateLastAccessedTime(string key, DateTime timestamp);

        /// <summary>
        /// 
        /// </summary>
        void Flush();


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Hashtable Load();
    }
}
