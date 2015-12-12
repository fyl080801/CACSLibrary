using System;

namespace CACSLibrary.Profile
{
    /// <summary>
    /// �����ļ����ܴ���
    /// </summary>
    /// <remarks>
    /// �������л��������͵� <see cref="CACSLibrary.Profile.IProfileProvider"/> ������������л�����
    /// </remarks>
    public interface IProfileEncryptionProvider
    {
        /// <summary>
        /// ����
        /// </summary>
        /// <param name="data">����</param>
        /// <returns>���ܺ������</returns>
        byte[] Encrypt(byte[] data);

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="data">����</param>
        /// <returns>���ܺ������</returns>
        byte[] Dencrypt(byte[] data);
    }
}
