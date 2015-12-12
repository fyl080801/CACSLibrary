using System;
using System.Collections;

namespace CACSLibrary.Caching
{
    /// <summary>
    /// 
    /// </summary>
    public class ExpirationTask
    {
        ICacheOperations cacheOperations;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheOperations"></param>
        public ExpirationTask(ICacheOperations cacheOperations)
        {
            this.cacheOperations = cacheOperations;
        }

        /// <summary>
        /// 
        /// </summary>
        public void DoExpirations()
        {
            Hashtable currentCacheState = this.cacheOperations.CurrentCacheState;
            this.MarkAsExpired(currentCacheState);
            this.SweepExpiredItemsFromCache(currentCacheState);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="liveCacheRepresentation"></param>
        /// <returns></returns>
        public virtual int MarkAsExpired(Hashtable liveCacheRepresentation)
        {
            if (liveCacheRepresentation == null)
            {
                throw new ArgumentNullException("liveCacheRepresentation");
            }
            int num = 0;
            foreach (CacheItem cacheItem in liveCacheRepresentation.Values)
            {
                lock (cacheItem)
                {
                    if (cacheItem.HasExpired())
                    {
                        num++;
                        cacheItem.WillBeExpired = true;
                    }
                }
            }
            return num;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="liveCacheRepresentation"></param>
        /// <returns></returns>
        public virtual int SweepExpiredItemsFromCache(Hashtable liveCacheRepresentation)
        {
            if (liveCacheRepresentation == null)
            {
                throw new ArgumentNullException("liveCacheRepresentation");
            }
            int num = 0;
            foreach (CacheItem itemToRemove in liveCacheRepresentation.Values)
            {
                if (this.RemoveItemFromCache(itemToRemove))
                {
                    num++;
                }
            }
            return num;
        }

        private bool RemoveItemFromCache(CacheItem itemToRemove)
        {
            bool result = false;
            lock (itemToRemove)
            {
                if (itemToRemove.WillBeExpired)
                {
                    try
                    {
                        result = true;
                        this.cacheOperations.RemoveItemFromCache(itemToRemove.Key, CacheItemRemovedReason.Expired);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            return result;
        }
    }
}
