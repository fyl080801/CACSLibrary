using System;
using System.Collections;

namespace CACSLibrary.Caching.BackingStoreImplementations
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class BaseBackingStore : IBackingStore, IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        public abstract int Count { get; }

        /// <summary>
        /// 
        /// </summary>
        ~BaseBackingStore()
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
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            this.Remove(key.GetHashCode());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="storageKey"></param>
        protected abstract void Remove(int storageKey);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="timestamp"></param>
        public void UpdateLastAccessedTime(string key, DateTime timestamp)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            this.UpdateLastAccessedTime(key.GetHashCode(), timestamp);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storageKey"></param>
        /// <param name="timestamp"></param>
        protected abstract void UpdateLastAccessedTime(int storageKey, DateTime timestamp);

        /// <summary>
        /// 
        /// </summary>
        public abstract void Flush();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newCacheItem"></param>
        public virtual void Add(CacheItem newCacheItem)
        {
            if (newCacheItem == null)
            {
                throw new ArgumentNullException("newCacheItem");
            }
            try
            {
                this.RemoveOldItem(newCacheItem.Key.GetHashCode());
                this.AddNewItem(newCacheItem.Key.GetHashCode(), newCacheItem);
            }
            catch
            {
                this.Remove(newCacheItem.Key.GetHashCode());
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual Hashtable Load()
        {
            return this.LoadDataFromStore();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storageKey"></param>
        protected abstract void RemoveOldItem(int storageKey);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storageKey"></param>
        /// <param name="newItem"></param>
        protected abstract void AddNewItem(int storageKey, CacheItem newItem);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract Hashtable LoadDataFromStore();
    }
}
