using System;
namespace CACSLibrary.Caching
{
    /// <summary>
    /// 缓存过期设定
    /// </summary>
    public interface ICacheItemExpiration
    {
        /// <summary>
        /// 是否过期
        /// </summary>
        /// <returns></returns>
        bool HasExpired();

        /// <summary>
        /// 通知
        /// </summary>
        void Notify();

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="owningCacheItem"></param>
        void Initialize(CacheItem owningCacheItem);
    }
}
