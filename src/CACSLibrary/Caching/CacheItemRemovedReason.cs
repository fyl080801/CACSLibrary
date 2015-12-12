using System;
namespace CACSLibrary.Caching
{
    /// <summary>
    /// 移除原因
    /// </summary>
    public enum CacheItemRemovedReason
    {
        /// <summary>
        /// 过期
        /// </summary>
        Expired,

        /// <summary>
        /// 移除
        /// </summary>
        Removed,

        /// <summary>
        /// ???
        /// </summary>
        Scavenged,

        /// <summary>
        /// 未知
        /// </summary>
        Unknown = 9999
    }
}
