using System;
namespace CACSLibrary.Caching
{
    /// <summary>
    /// �Ƴ�ԭ��
    /// </summary>
    public enum CacheItemRemovedReason
    {
        /// <summary>
        /// ����
        /// </summary>
        Expired,

        /// <summary>
        /// �Ƴ�
        /// </summary>
        Removed,

        /// <summary>
        /// ???
        /// </summary>
        Scavenged,

        /// <summary>
        /// δ֪
        /// </summary>
        Unknown = 9999
    }
}
