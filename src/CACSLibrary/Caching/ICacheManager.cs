using System;

namespace CACSLibrary.Caching
{
    /// <summary>
    /// 缓存管理
    /// </summary>
    public interface ICacheManager
    {
        /// <summary>
        /// 缓存对象数
        /// </summary>
        int Count
        {
            get;
        }

        /// <summary>
        /// 根据键值返回缓存对象
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        object this[string key]
        {
            get;
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        void Add(string key, object value);

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="scavengingPriority">缓存优先级</param>
        /// <param name="refreshAction">刷新行为</param>
        /// <param name="expirations">过期设定</param>
        void Add(string key, object value, CacheItemPriority scavengingPriority, ICacheItemRefreshAction refreshAction, params ICacheItemExpiration[] expirations);

        /// <summary>
        /// 包含检测
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>是否包含</returns>
        /// <remarks>检测缓存是否包含指定项</remarks>
        bool Contains(string key);

        /// <summary>
        /// 释放
        /// </summary>
        void Flush();

        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        object GetData(string key);

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="key">键</param>
        void Remove(string key);
    }
}
