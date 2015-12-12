using System;

namespace CACSLibrary.Caching.Expirations
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class SlidingTime : ICacheItemExpiration
    {
        DateTime timeLastUsed;
        TimeSpan itemSlidingExpiration;

        /// <summary>
        /// 
        /// </summary>
        public TimeSpan ItemSlidingExpiration
        {
            get { return this.itemSlidingExpiration; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime TimeLastUsed
        {
            get
            {
                return this.timeLastUsed;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="slidingExpiration"></param>
        public SlidingTime(TimeSpan slidingExpiration)
        {
            double arg_17_0 = slidingExpiration.TotalSeconds;
            this.itemSlidingExpiration = slidingExpiration;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="slidingExpiration"></param>
        /// <param name="originalTimeStamp"></param>
        public SlidingTime(TimeSpan slidingExpiration, DateTime originalTimeStamp)
            : this(slidingExpiration)
        {
            this.timeLastUsed = originalTimeStamp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool HasExpired()
        {
            return SlidingTime.CheckSlidingExpiration(DateTime.Now, this.timeLastUsed, this.itemSlidingExpiration);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Notify()
        {
            this.timeLastUsed = DateTime.Now;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owningCacheItem"></param>
        public void Initialize(CacheItem owningCacheItem)
        {
            if (owningCacheItem == null)
            {
                throw new ArgumentNullException("owningCacheItem");
            }
            this.timeLastUsed = owningCacheItem.LastAccessedTime;
        }

        private static bool CheckSlidingExpiration(DateTime nowDateTime, DateTime lastUsed, TimeSpan slidingExpiration)
        {
            DateTime dateTime = nowDateTime.ToUniversalTime();
            long num = lastUsed.ToUniversalTime().Ticks + slidingExpiration.Ticks;
            return dateTime.Ticks >= num;
        }
    }
}
