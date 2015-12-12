using System;
using System.Collections;
using System.Threading;

namespace CACSLibrary.Caching
{
    /// <summary>
    /// 
    /// </summary>
    public class Cache : ICacheOperations, IDisposable
    {
        const string addInProgressFlag = "Dummy variable used to flag temp cache item added during Add";
        Hashtable inMemoryCache;
        IBackingStore backingStore;

        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get { return this.inMemoryCache.Count; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Hashtable CurrentCacheState
        {
            get { return (Hashtable)this.inMemoryCache.Clone(); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="backingStore"></param>
        public Cache(IBackingStore backingStore)
        {
            if (backingStore == null)
            {
                throw new ArgumentNullException("backingStore");
            }
            this.backingStore = backingStore;
            Hashtable table = backingStore.Load();
            this.inMemoryCache = Hashtable.Synchronized(table);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Contains(string key)
        {
            Cache.ValidateKey(key);
            return this.inMemoryCache.Contains(key);
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
            Cache.ValidateKey(key);
            CacheItem cacheItem = null;
            bool flag = false;
            do
            {
                lock (this.inMemoryCache.SyncRoot)
                {
                    if (!this.inMemoryCache.Contains(key))
                    {
                        cacheItem = new CacheItem(key, "Dummy variable used to flag temp cache item added during Add", CacheItemPriority.NotRemovable, null, new ICacheItemExpiration[0]);
                        this.inMemoryCache[key] = cacheItem;
                    }
                    else
                    {
                        cacheItem = (CacheItem)this.inMemoryCache[key];
                    }
                    flag = Monitor.TryEnter(cacheItem);
                }
                if (!flag)
                {
                    Thread.Sleep(0);
                }
            }
            while (!flag);
            try
            {
                cacheItem.TouchedByUserAction(true);
                CacheItem newCacheItem = new CacheItem(key, value, scavengingPriority, refreshAction, expirations);
                try
                {
                    this.backingStore.Add(newCacheItem);
                    cacheItem.Replace(value, refreshAction, scavengingPriority, expirations);
                    this.inMemoryCache[key] = cacheItem;
                }
                catch
                {
                    this.backingStore.Remove(key);
                    this.inMemoryCache.Remove(key);
                    throw;
                }
            }
            finally
            {
                Monitor.Exit(cacheItem);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            this.Remove(key, CacheItemRemovedReason.Removed);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="removalReason"></param>
        public void Remove(string key, CacheItemRemovedReason removalReason)
        {
            Cache.ValidateKey(key);
            CacheItem cacheItem = null;
            bool flag2;
            do
            {
                lock (this.inMemoryCache.SyncRoot)
                {
                    cacheItem = (CacheItem)this.inMemoryCache[key];
                    if (Cache.IsObjectInCache(cacheItem))
                    {
                        return;
                    }
                    flag2 = Monitor.TryEnter(cacheItem);
                }
                if (!flag2)
                {
                    Thread.Sleep(0);
                }
            }
            while (!flag2);
            try
            {
                cacheItem.TouchedByUserAction(true);
                this.backingStore.Remove(key);
                this.inMemoryCache.Remove(key);
                RefreshActionInvoker.InvokeRefreshAction(cacheItem, removalReason);
            }
            finally
            {
                Monitor.Exit(cacheItem);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="removalReason"></param>
        public void RemoveItemFromCache(string key, CacheItemRemovedReason removalReason)
        {
            this.Remove(key, removalReason);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetData(string key)
        {
            Cache.ValidateKey(key);
            CacheItem cacheItem = null;
            bool flag = false;
            object result;
            do
            {
                lock (this.inMemoryCache.SyncRoot)
                {
                    cacheItem = (CacheItem)this.inMemoryCache[key];
                    if (Cache.IsObjectInCache(cacheItem))
                    {
                        result = null;
                        return result;
                    }
                    flag = Monitor.TryEnter(cacheItem);
                }
                if (!flag)
                {
                    Thread.Sleep(0);
                }
            }
            while (!flag);
            try
            {
                if (cacheItem.HasExpired())
                {
                    cacheItem.TouchedByUserAction(true);
                    this.backingStore.Remove(key);
                    this.inMemoryCache.Remove(key);
                    RefreshActionInvoker.InvokeRefreshAction(cacheItem, CacheItemRemovedReason.Expired);
                    result = null;
                }
                else
                {
                    this.backingStore.UpdateLastAccessedTime(cacheItem.Key, DateTime.Now);
                    cacheItem.TouchedByUserAction(false);
                    result = cacheItem.Value;
                }
            }
            finally
            {
                Monitor.Exit(cacheItem);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Flush()
        {
            while (true)
            {
            IL_00:
                lock (this.inMemoryCache.SyncRoot)
                {
                    foreach (string key in this.inMemoryCache.Keys)
                    {
                        bool flag2 = false;
                        CacheItem cacheItem = (CacheItem)this.inMemoryCache[key];
                        try
                        {
                            if (!(flag2 = Monitor.TryEnter(cacheItem)))
                            {
                                goto IL_00;
                            }
                            cacheItem.TouchedByUserAction(true);
                        }
                        finally
                        {
                            if (flag2)
                            {
                                Monitor.Exit(cacheItem);
                            }
                        }
                    }
                    int arg_98_0 = this.inMemoryCache.Count;
                    this.backingStore.Flush();
                    this.inMemoryCache.Clear();
                }
                break;
            }
        }

        private static void ValidateKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }
        }

        private static bool IsObjectInCache(CacheItem cacheItemBeforeLock)
        {
            return cacheItemBeforeLock == null || object.ReferenceEquals(cacheItemBeforeLock.Value, "Dummy variable used to flag temp cache item added during Add");
        }

        /// <summary>
        /// 
        /// </summary>
        ~Cache()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.backingStore.Dispose();
                this.backingStore = null;
            }
        }
    }
}
