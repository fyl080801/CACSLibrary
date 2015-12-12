using System;

namespace CACSLibrary.Profile
{
    /// <summary>
    /// 配置文件加密处理
    /// </summary>
    /// <remarks>
    /// 用于序列化数据类型的 <see cref="CACSLibrary.Profile.IProfileProvider"/> 加密与解密序列化数据
    /// </remarks>
    public interface IProfileEncryptionProvider
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>加密后的数据</returns>
        byte[] Encrypt(byte[] data);

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>解密后的数据</returns>
        byte[] Dencrypt(byte[] data);
    }
}
