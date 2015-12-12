using System;

namespace CACSLibrary.Caching
{
    /// <summary>
    /// �������
    /// </summary>
    public interface ICacheManager
    {
        /// <summary>
        /// ���������
        /// </summary>
        int Count
        {
            get;
        }

        /// <summary>
        /// ���ݼ�ֵ���ػ������
        /// </summary>
        /// <param name="key">��</param>
        /// <returns></returns>
        object this[string key]
        {
            get;
        }

        /// <summary>
        /// ��ӻ���
        /// </summary>
        /// <param name="key">��</param>
        /// <param name="value">ֵ</param>
        void Add(string key, object value);

        /// <summary>
        /// ��ӻ���
        /// </summary>
        /// <param name="key">��</param>
        /// <param name="value">ֵ</param>
        /// <param name="scavengingPriority">�������ȼ�</param>
        /// <param name="refreshAction">ˢ����Ϊ</param>
        /// <param name="expirations">�����趨</param>
        void Add(string key, object value, CacheItemPriority scavengingPriority, ICacheItemRefreshAction refreshAction, params ICacheItemExpiration[] expirations);

        /// <summary>
        /// �������
        /// </summary>
        /// <param name="key">��</param>
        /// <returns>�Ƿ����</returns>
        /// <remarks>��⻺���Ƿ����ָ����</remarks>
        bool Contains(string key);

        /// <summary>
        /// �ͷ�
        /// </summary>
        void Flush();

        /// <summary>
        /// ��ȡ��������
        /// </summary>
        /// <param name="key">��</param>
        /// <returns></returns>
        object GetData(string key);

        /// <summary>
        /// �Ƴ�
        /// </summary>
        /// <param name="key">��</param>
        void Remove(string key);
    }
}
