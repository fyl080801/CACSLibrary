using System;

namespace CACSLibrary.Profile
{
    /// <summary>
    /// 不加密处理配置文件序列化数据
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
