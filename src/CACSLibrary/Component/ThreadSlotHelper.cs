using System;
using System.Threading;

namespace CACSLibrary.Component
{
    /// <summary>
    /// �̲߳۴�����
    /// </summary>
	public class ThreadSlotHelper
	{
        /// <summary>
        /// �������ʵ�����̲߳�
        /// </summary>
        /// <param name="key">��</param>
        /// <param name="instance">ʵ��</param>
		public static void SaveInstanceToStore(string key, object instance)
		{
			Thread.SetData(ThreadSlotHelper.GetSlot(key), instance);
		}

        /// <summary>
        /// ���̲߳ۻ�ȡʵ��
        /// </summary>
        /// <param name="datakey">��</param>
        /// <returns>ʵ��</returns>
		public static object GetInstanceFromStore(string datakey)
		{
			return Thread.GetData(ThreadSlotHelper.GetSlot(datakey));
		}

		private static LocalDataStoreSlot GetSlot(string datakey)
		{
			return Thread.GetNamedDataSlot(datakey);
		}

        /// <summary>
        /// ���̲߳�ж��ʵ��
        /// </summary>
        /// <param name="datakey">��</param>
		public static void Unload(string datakey)
		{
			Thread.FreeNamedDataSlot(datakey);
		}
	}
}
