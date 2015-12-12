using System;
namespace CACSLibrary.Caching
{
    /// <summary>
    /// ˢ����Ϊ
    /// </summary>
    public interface ICacheItemRefreshAction
    {
        /// <summary>
        /// ˢ��
        /// </summary>
        /// <param name="removedKey">�Ƴ��ļ�</param>
        /// <param name="expiredValue">���ڵ�ֵ</param>
        /// <param name="removalReason">�Ƴ�ԭ��</param>
        void Refresh(string removedKey, object expiredValue, CacheItemRemovedReason removalReason);
    }
}
