using System;
using System.Collections;

namespace CACSLibrary.Caching
{
    /// <summary>
    /// 
    /// </summary>
    public class ScavengerTask
    {
        readonly int maximumElementsInCacheBeforeScavenging;
        readonly int numberToRemoveWhenScavenging;
        ICacheOperations cacheOperations;

        internal int NumberOfItemsToBeScavenged
        {
            get
            {
                return this.numberToRemoveWhenScavenging;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numberToRemoveWhenScavenging"></param>
        /// <param name="maximumElementsInCacheBeforeScavenging"></param>
        /// <param name="cacheOperations"></param>
        public ScavengerTask(int numberToRemoveWhenScavenging, int maximumElementsInCacheBeforeScavenging, ICacheOperations cacheOperations)
        {
            this.numberToRemoveWhenScavenging = numberToRemoveWhenScavenging;
            this.maximumElementsInCacheBeforeScavenging = maximumElementsInCacheBeforeScavenging;
            this.cacheOperations = cacheOperations;
        }

        /// <summary>
        /// 
        /// </summary>
        public void DoScavenging()
        {
            if (this.NumberOfItemsToBeScavenged == 0)
            {
                return;
            }
            if (this.IsScavengingNeeded())
            {
                Hashtable currentCacheState = this.cacheOperations.CurrentCacheState;
                ScavengerTask.ResetScavengingFlagInCacheItems(currentCacheState);
                SortedList scavengableItems = ScavengerTask.SortItemsForScavenging(currentCacheState);
                this.RemoveScavengableItems(scavengableItems);
            }
        }

        private static void ResetScavengingFlagInCacheItems(Hashtable liveCacheRepresentation)
        {
            foreach (CacheItem cacheItem in liveCacheRepresentation.Values)
            {
                lock (cacheItem)
                {
                    cacheItem.MakeEligibleForScavenging();
                }
            }
        }

        private static SortedList SortItemsForScavenging(Hashtable unsortedItemsInCache)
        {
            return new SortedList(unsortedItemsInCache, new PriorityDateComparer(unsortedItemsInCache));
        }

        private void RemoveScavengableItems(SortedList scavengableItems)
        {
            int num = 0;
            foreach (CacheItem itemToRemove in scavengableItems.Values)
            {
                bool flag = this.RemoveItemFromCache(itemToRemove);
                if (flag)
                {
                    num++;
                    if (num == this.NumberOfItemsToBeScavenged)
                    {
                        break;
                    }
                }
            }
        }

        private bool RemoveItemFromCache(CacheItem itemToRemove)
        {
            lock (itemToRemove)
            {
                if (itemToRemove.EligibleForScavenging)
                {
                    try
                    {
                        this.cacheOperations.RemoveItemFromCache(itemToRemove.Key, CacheItemRemovedReason.Scavenged);
                        return true;
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            return false;
        }

        internal bool IsScavengingNeeded()
        {
            return this.cacheOperations.Count > this.maximumElementsInCacheBeforeScavenging;
        }
    }
}
