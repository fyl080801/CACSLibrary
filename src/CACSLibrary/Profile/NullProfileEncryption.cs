using System;

namespace CACSLibrary.Profile
{
    /// <summary>
    /// �����ܴ��������ļ����л�����
    /// </summary>
    public class NullProfileEncryption : IProfileEncryptionProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] Encrypt(byte[] data)
        {
            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] Dencrypt(byte[] data)
        {
            return data;
        }
    }
}
