using System;
namespace CACSLibrary.Caching
{
    /// <summary>
    /// ��������趨
    /// </summary>
    public interface ICacheItemExpiration
    {
        /// <summary>
        /// �Ƿ����
        /// </summary>
        /// <returns></returns>
        bool HasExpired();

        /// <summary>
        /// ֪ͨ
        /// </summary>
        void Notify();

        /// <summary>
        /// ��ʼ��
        /// </summary>
        /// <param name="owningCacheItem"></param>
        void Initialize(CacheItem owningCacheItem);
    }
}
