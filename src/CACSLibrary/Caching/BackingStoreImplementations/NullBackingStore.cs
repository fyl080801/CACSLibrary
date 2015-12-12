using System;
using System.Collections;

namespace CACSLibrary.Caching.BackingStoreImplementations
{
    /// <summary>
    /// 
    /// </summary>
    public class NullBackingStore : IBackingStore, IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get { return 0; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newCacheItem"></param>
        public void Add(CacheItem newCacheItem)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="timestamp"></param>
        public void UpdateLastAccessedTime(string key, DateTime timestamp)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public void Flush()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Hashtable Load()
        {
            return new Hashtable();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
        }
    }
}
