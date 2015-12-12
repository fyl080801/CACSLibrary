using System;

namespace CACSLibrary.Caching.Expirations
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class AbsoluteTime : ICacheItemExpiration
    {
        DateTime absoluteExpirationTime;

        /// <summary>
        /// 
        /// </summary>
        public DateTime AbsoluteExpirationTime
        {
            get { return this.absoluteExpirationTime; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="absoluteTime"></param>
        public AbsoluteTime(DateTime absoluteTime)
        {
            if (absoluteTime > DateTime.Now)
            {
                this.absoluteExpirationTime = absoluteTime.ToUniversalTime();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeFromNow"></param>
        public AbsoluteTime(TimeSpan timeFromNow)
            : this(DateTime.Now + timeFromNow)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool HasExpired()
        {
            return DateTime.Now.ToUniversalTime().Ticks >= this.absoluteExpirationTime.Ticks;
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
