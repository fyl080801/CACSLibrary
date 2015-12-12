using System;
using System.Collections;

namespace CACSLibrary.Caching
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICacheOperations
    {
        /// <summary>
        /// 
        /// </summary>
        Hashtable CurrentCacheState { get; }

        /// <summary>
        /// 
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="removalReason"></param>
        void RemoveItemFromCache(string key, CacheItemRemovedReason removalReason);
    }
}
