using System;

namespace CACSLibrary.Profile
{
    /// <summary>
    /// �����ļ�����ӿ�
    /// </summary>
    /// <remarks>
    /// �˽ӿڶ�����һ�������ļ��Ļ�����������ͨ��ʵ��</remarks>
	public interface IProfileProvider
	{
        /// <summary>
        /// ��ȡ�����ļ�
        /// </summary>
        /// <param name="configType">�����ļ�����</param>
        /// <returns>�����ļ�����</returns>
		object Load(Type configType);

        /// <summary>
        /// ���������ļ�
        /// </summary>
        /// <param name="config">�����ļ�����</param>
		void Save(object config);

        /// <summary>
        /// ��������ļ�
        /// </summary>
        /// <param name="config">�����ļ�����</param>
		void Clear(object config);
	}
}
