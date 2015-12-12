using System;
using System.Collections;

namespace CACSLibrary.Caching.BackingStoreImplementations
{
    /// <summary>
    /// 
    /// </summary>
    public class StaticBackingStore : BaseBackingStore
    {
        static Hashtable _hash;
        static readonly object lockHelper = new object();

        /// <summary>
        /// 
        /// </summary>
        public override int Count
        {
            get
            {
                return StaticBackingStore._hash.Count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public StaticBackingStore()
        {
            if (StaticBackingStore._hash == null)
            {
                lock (StaticBackingStore.lockHelper)
                {
                    if (StaticBackingStore._hash == null)
                    {
                        StaticBackingStore._hash = new Hashtable();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storageKey"></param>
        protected override void Remove(int storageKey)
        {
            StaticBackingStore._hash.Remove(storageKey);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storageKey"></param>
        /// <param name="timestamp"></param>
        protected override void UpdateLastAccessedTime(int storageKey, DateTime timestamp)
        {
            CacheItem cacheItem = StaticBackingStore._hash[storageKey] as CacheItem;
            cacheItem.SetLastAccessedTime(timestamp);
            StaticBackingStore._hash[storageKey] = cacheItem;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Flush()
        {
            StaticBackingStore._hash.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storageKey"></param>
        protected override void RemoveOldItem(int storageKey)
        {
            StaticBackingStore._hash.Remove(storageKey);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storageKey"></param>
        /// <param name="newItem"></param>
        protected override void AddNewItem(int storageKey, CacheItem newItem)
        {
            StaticBackingStore._hash.Add(storageKey, newItem);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override Hashtable LoadDataFromStore()
        {
            return StaticBackingStore._hash;
        }
    }
}
