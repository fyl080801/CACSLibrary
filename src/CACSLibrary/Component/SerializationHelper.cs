using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace CACSLibrary.Component
{
	/// <summary>
	/// 序列化工具类
	/// </summary>
	public static class SerializationHelper
	{
		/// <summary>
		/// 将对象序列化成二进制数组
		/// </summary>
		/// <param name="value">对象</param>
		/// <returns>二进制数组</returns>
		public static byte[] ToBytes(object value)
		{
			if (value == null)
			{
				return null;
			}
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				new BinaryFormatter().Serialize(memoryStream, value);
				result = memoryStream.ToArray();
			}
			return result;
		}

		/// <summary>
		/// 将二进制数组还原成对象
		/// </summary>
		/// <param name="serializedObject">二进制数组</param>
		/// <returns>对象</returns>
		public static object ToObject(byte[] serializedObject)
		{
			if (serializedObject == null)
			{
				return null;
			}
			object result;
			using (MemoryStream memoryStream = new MemoryStream(serializedObject))
			{
				result = new BinaryFormatter().Deserialize(memoryStream);
			}
			return result;
		}
	}
}
