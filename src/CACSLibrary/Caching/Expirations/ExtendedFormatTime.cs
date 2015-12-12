using System;
using System.Runtime.InteropServices;

namespace CACSLibrary.Caching.Expirations
{
    /// <summary>
    /// 
    /// </summary>
    [ComVisible(false)]
    [Serializable]
    public class ExtendedFormatTime : ICacheItemExpiration
    {
        string extendedFormat;
        DateTime lastUsedTime;

        /// <summary>
        /// 
        /// </summary>
        public string TimeFormat
        {
            get { return this.extendedFormat; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeFormat"></param>
        public ExtendedFormatTime(string timeFormat)
        {
            string.IsNullOrEmpty(timeFormat);
            ExtendedFormat.Validate(timeFormat);
            this.extendedFormat = timeFormat;
            this.lastUsedTime = DateTime.Now.ToUniversalTime();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool HasExpired()
        {
            DateTime nowTime = DateTime.Now.ToUniversalTime();
            ExtendedFormat extendedFormat = new ExtendedFormat(this.extendedFormat);
            return extendedFormat.IsExpired(this.lastUsedTime, nowTime);
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
