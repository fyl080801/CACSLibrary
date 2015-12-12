using System;
namespace CACSLibrary.Caching
{
    /// <summary>
    /// 缓存优先级
    /// </summary>
    public enum CacheItemPriority
    {
        /// <summary>
        /// 无
        /// </summary>
        None,

        /// <summary>
        /// 低
        /// </summary>
        Low,

        /// <summary>
        /// 中
        /// </summary>
        Normal,

        /// <summary>
        /// 高
        /// </summary>
        High,

        /// <summary>
        /// 不可移除
        /// </summary>
        NotRemovable
    }
}
