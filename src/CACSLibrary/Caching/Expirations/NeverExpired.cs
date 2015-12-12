using System;

namespace CACSLibrary.Caching.Expirations
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class NeverExpired : ICacheItemExpiration
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool HasExpired()
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Notify()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owningCacheItem"></param>
        public void Initialize(CacheItem owningCacheItem)
        {
        }
    }
}
