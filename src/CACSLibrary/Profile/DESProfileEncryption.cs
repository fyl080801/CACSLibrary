using CACSLibrary.Component;
using System;
namespace CACSLibrary.Profile
{
    /// <summary>
    /// �� DES ��ʽ���ܽ������л����ݵ������ļ�����
    /// </summary>
    public class DESProfileEncryption : IProfileEncryptionProvider
    {
        byte[] _key;
        byte[] _iv;

        /// <summary>
        /// ��ʼ����
        /// </summary>
        /// <param name="key">��Կ</param>
        /// <param name="iv">iv</param>
        public DESProfileEncryption(byte[] key, byte[] iv)
        {
            this._key = key;
            this._iv = iv;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] Encrypt(byte[] data)
        {
            CryptHelper.EncryptDES(ref data, this._key, this._iv);
            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] Dencrypt(byte[] data)
        {
            CryptHelper.DecryptDES(ref data, this._key, this._iv);
            return data;
        }
    }
}
