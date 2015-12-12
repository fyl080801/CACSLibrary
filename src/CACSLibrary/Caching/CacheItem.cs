using CACSLibrary.Caching.Expirations;
using System;
namespace CACSLibrary.Caching
{
    /// <summary>
    /// »º´æÏî
    /// </summary>
    public class CacheItem
    {
        string key;
        object data;
        ICacheItemRefreshAction refreshAction;
        ICacheItemExpiration[] expirations;
        CacheItemPriority scavengingPriority;
        DateTime lastAccessedTime;
        bool willBeExpired;
        bool eligibleForScavenging;

        /// <summary>
        /// 
        /// </summary>
        public CacheItemPriority ScavengingPriority
        {
            get { return this.scavengingPriority; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime LastAccessedTime
        {
            get { return this.lastAccessedTime; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool WillBeExpired
        {
            get { return this.willBeExpired; }
            set { this.willBeExpired = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool EligibleForScavenging
        {
            get
            {
                return this.eligibleForScavenging && this.ScavengingPriority != CacheItemPriority.NotRemovable;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public object Value
        {
            get { return this.data; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Key
        {
            get { return this.key; }
        }

        /// <summary>
        /// 
        /// </summary>
        public ICacheItemRefreshAction RefreshAction
        {
            get { return this.refreshAction; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="scavengingPriority"></param>
        /// <param name="refreshAction"></param>
        /// <param name="expirations"></param>
        public CacheItem(string key, object value, CacheItemPriority scavengingPriority, ICacheItemRefreshAction refreshAction, params ICacheItemExpiration[] expirations)
        {
            this.Initialize(key, value, refreshAction, scavengingPriority, expirations);
            this.TouchedByUserAction(false);
            this.InitializeExpirations();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lastAccessedTime"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="scavengingPriority"></param>
        /// <param name="refreshAction"></param>
        /// <param name="expirations"></param>
        public CacheItem(DateTime lastAccessedTime, string key, object value, CacheItemPriority scavengingPriority, ICacheItemRefreshAction refreshAction, params ICacheItemExpiration[] expirations)
        {
            this.Initialize(key, value, refreshAction, scavengingPriority, expirations);
            this.TouchedByUserAction(false, lastAccessedTime);
            this.InitializeExpirations();
        }

        internal void Replace(object cacheItemData, ICacheItemRefreshAction cacheItemRefreshAction, CacheItemPriority cacheItemPriority, params ICacheItemExpiration[] cacheItemExpirations)
        {
            this.Initialize(this.key, cacheItemData, cacheItemRefreshAction, cacheItemPriority, cacheItemExpirations);
            this.TouchedByUserAction(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ICacheItemExpiration[] GetExpirations()
        {
            return (ICacheItemExpiration[])this.expirations.Clone();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool HasExpired()
        {
            ICacheItemExpiration[] array = this.expirations;
            for (int i = 0; i < array.Length; i++)
            {
                ICacheItemExpiration cacheItemExpiration = array[i];
                if (cacheItemExpiration.HasExpired())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectRemovedFromCache"></param>
        public void TouchedByUserAction(bool objectRemovedFromCache)
        {
            this.TouchedByUserAction(objectRemovedFromCache, DateTime.Now);
        }

        internal void TouchedByUserAction(bool objectRemovedFromCache, DateTime timestamp)
        {
            this.lastAccessedTime = timestamp;
            this.eligibleForScavenging = false;
            ICacheItemExpiration[] array = this.expirations;
            for (int i = 0; i < array.Length; i++)
            {
                ICacheItemExpiration cacheItemExpiration = array[i];
                cacheItemExpiration.Notify();
            }
            this.willBeExpired = (!objectRemovedFromCache && this.HasExpired());
        }

        /// <summary>
        /// 
        /// </summary>
        public void MakeEligibleForScavenging()
        {
            this.eligibleForScavenging = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void MakeNotEligibleForScavenging()
        {
            this.eligibleForScavenging = false;
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitializeExpirations()
        {
            ICacheItemExpiration[] array = this.expirations;
            for (int i = 0; i < array.Length; i++)
            {
                ICacheItemExpiration cacheItemExpiration = array[i];
                cacheItemExpiration.Initialize(this);
            }
        }

        private void Initialize(string cacheItemKey, object cacheItemData, ICacheItemRefreshAction cacheItemRefreshAction, CacheItemPriority cacheItemPriority, ICacheItemExpiration[] cacheItemExpirations)
        {
            this.key = cacheItemKey;
            this.data = cacheItemData;
            this.refreshAction = cacheItemRefreshAction;
            this.scavengingPriority = cacheItemPriority;
            if (cacheItemExpirations == null)
            {
                this.expirations = new ICacheItemExpiration[]
				{
					new NeverExpired()
				};
                return;
            }
            this.expirations = cacheItemExpirations;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="specificAccessedTime"></param>
        public void SetLastAccessedTime(DateTime specificAccessedTime)
        {
            this.lastAccessedTime = specificAccessedTime;
        }
    }
}
