using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CACSLibrary.Component
{
    /// <summary>
    /// 加密处理工具类
    /// </summary>
	public class CryptHelper
	{
		private static DES _DES;

        /// <summary>
        /// 
        /// </summary>
		public static DES DES
		{
			get
			{
				if (CryptHelper._DES == null)
				{
					CryptHelper._DES = new DESCryptoServiceProvider();
				}
				return CryptHelper._DES;
			}
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
		public static string MD5Encoder(string str)
		{
			MD5 mD = MD5.Create();
			byte[] array = mD.ComputeHash(Encoding.Unicode.GetBytes(str));
			string text = "";
			byte[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				byte b = array2[i];
				text += b.ToString("X");
			}
			return text;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
		public static void EncryptDES(ref byte[] data, byte[] key, byte[] iv)
		{
			try
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, CryptHelper.DES.CreateEncryptor(key, iv), CryptoStreamMode.Write))
					{
						cryptoStream.Write(data, 0, data.Length);
						cryptoStream.FlushFinalBlock();
						data = memoryStream.ToArray();
					}
				}
			}
			catch
			{
				throw;
			}
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
		public static void DecryptDES(ref byte[] data, byte[] key, byte[] iv)
		{
			try
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, CryptHelper.DES.CreateDecryptor(key, iv), CryptoStreamMode.Write))
					{
						cryptoStream.Write(data, 0, data.Length);
						cryptoStream.FlushFinalBlock();
						data = memoryStream.ToArray();
					}
				}
			}
			catch
			{
				throw;
			}
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
		public static string StringEncryptDES(string data, byte[] key, byte[] iv)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(data);
			CryptHelper.EncryptDES(ref bytes, key, iv);
			return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
		public static string StringDecryptDES(string data, byte[] key, byte[] iv)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(data);
			CryptHelper.DecryptDES(ref bytes, key, iv);
			return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
		}
	}
}
