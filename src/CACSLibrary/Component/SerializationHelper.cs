using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace CACSLibrary.Component
{
	/// <summary>
	/// ���л�������
	/// </summary>
	public static class SerializationHelper
	{
		/// <summary>
		/// ���������л��ɶ���������
		/// </summary>
		/// <param name="value">����</param>
		/// <returns>����������</returns>
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
		/// �����������黹ԭ�ɶ���
		/// </summary>
		/// <param name="serializedObject">����������</param>
		/// <returns>����</returns>
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
