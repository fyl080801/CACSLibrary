using System;
using System.Threading;

namespace CACSLibrary.Caching
{
    /// <summary>
    /// 
    /// </summary>
    public class CacheManager : IDisposable, ICacheManager
    {
        const string DATAKEY = "_CACHEMANAGER";
        const string AREAKEY = "_CACHESTORE";
        Cache realCache;
        ExpirationPollTimer pollTimer;
        readonly BackgroundScheduler backgroundScheduler;

        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get { return this.realCache.Count; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object this[string key]
        {
            get { return this.realCache.GetData(key); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="realCache"></param>
        /// <param name="backgroundScheduler"></param>
        /// <param name="pollTimer"></param>
        public CacheManager(Cache realCache, BackgroundScheduler backgroundScheduler, ExpirationPollTimer pollTimer)
        {
            if (pollTimer == null)
            {
                throw new ArgumentNullException("pollTimer");
            }
            this.realCache = realCache;
            this.pollTimer = pollTimer;
            this.backgroundScheduler = backgroundScheduler;
            pollTimer.StartPolling(new TimerCallback(backgroundScheduler.ExpirationTimeoutExpired));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Contains(string key)
        {
            return this.realCache.Contains(key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, object value)
        {
            this.Add(key, value, CacheItemPriority.Normal, null, new ICacheItemExpiration[0]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="scavengingPriority"></param>
        /// <param name="refreshAction"></param>
        /// <param name="expirations"></param>
        public void Add(string key, object value, CacheItemPriority scavengingPriority, ICacheItemRefreshAction refreshAction, params ICacheItemExpiration[] expirations)
        {
            this.realCache.Add(key, value, scavengingPriority, refreshAction, expirations);
            this.backgroundScheduler.StartScavengingIfNeeded();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            this.realCache.Remove(key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetData(string key)
        {
            return this.realCache.GetData(key);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Flush()
        {
            this.realCache.Flush();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            if (this.pollTimer != null)
            {
                this.pollTimer.StopPolling();
                this.pollTimer = null;
            }
            if (this.realCache != null)
            {
                this.realCache.Dispose();
                this.realCache = null;
            }
        }
    }
}
