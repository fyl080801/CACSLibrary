using System;
namespace CACSLibrary.Caching
{
    /// <summary>
    /// 刷新行为
    /// </summary>
    public interface ICacheItemRefreshAction
    {
        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="removedKey">移除的键</param>
        /// <param name="expiredValue">过期的值</param>
        /// <param name="removalReason">移除原因</param>
        void Refresh(string removedKey, object expiredValue, CacheItemRemovedReason removalReason);
    }
}
