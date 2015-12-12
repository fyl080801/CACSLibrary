using System;
using System.Threading;

namespace CACSLibrary.Caching
{
    /// <summary>
    /// 
    /// </summary>
    public static class RefreshActionInvoker
    {
        class RefreshActionData
        {
            ICacheItemRefreshAction refreshAction;
            string keyToRefresh;
            object removedData;
            CacheItemRemovedReason removalReason;

            public ICacheItemRefreshAction RefreshAction
            {
                get { return this.refreshAction; }
            }

            public string KeyToRefresh
            {
                get { return this.keyToRefresh; }
            }

            public CacheItemRemovedReason RemovalReason
            {
                get { return this.removalReason; }
            }

            public object RemovedData
            {
                get { return this.removedData; }
            }

            public RefreshActionData(ICacheItemRefreshAction refreshAction, string keyToRefresh, object removedData, CacheItemRemovedReason removalReason)
            {
                this.refreshAction = refreshAction;
                this.keyToRefresh = keyToRefresh;
                this.removalReason = removalReason;
                this.removedData = removedData;
            }

            public void InvokeOnThreadPoolThread()
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(this.ThreadPoolRefreshActionInvoker));
            }

            private void ThreadPoolRefreshActionInvoker(object notUsed)
            {
                try
                {
                    this.RefreshAction.Refresh(this.KeyToRefresh, this.RemovedData, this.RemovalReason);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="removedCacheItem"></param>
        /// <param name="removalReason"></param>
        public static void InvokeRefreshAction(CacheItem removedCacheItem, CacheItemRemovedReason removalReason)
        {
            if (removedCacheItem == null)
            {
                throw new ArgumentNullException("removedCacheItem");
            }
            if (removedCacheItem.RefreshAction == null)
            {
                return;
            }
            try
            {
                RefreshActionInvoker.RefreshActionData refreshActionData = new RefreshActionInvoker.RefreshActionData(removedCacheItem.RefreshAction, removedCacheItem.Key, removedCacheItem.Value, removalReason);
                refreshActionData.InvokeOnThreadPoolThread();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
