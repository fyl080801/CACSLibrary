using System;
using System.Threading;

namespace CACSLibrary.Component
{
    /// <summary>
    /// 线程槽处理类
    /// </summary>
	public class ThreadSlotHelper
	{
        /// <summary>
        /// 保存对象实例到线程槽
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="instance">实例</param>
		public static void SaveInstanceToStore(string key, object instance)
		{
			Thread.SetData(ThreadSlotHelper.GetSlot(key), instance);
		}

        /// <summary>
        /// 从线程槽获取实例
        /// </summary>
        /// <param name="datakey">键</param>
        /// <returns>实例</returns>
		public static object GetInstanceFromStore(string datakey)
		{
			return Thread.GetData(ThreadSlotHelper.GetSlot(datakey));
		}

		private static LocalDataStoreSlot GetSlot(string datakey)
		{
			return Thread.GetNamedDataSlot(datakey);
		}

        /// <summary>
        /// 从线程槽卸载实例
        /// </summary>
        /// <param name="datakey">键</param>
		public static void Unload(string datakey)
		{
			Thread.FreeNamedDataSlot(datakey);
		}
	}
}
