using System;

namespace CACSLibrary.Data
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class BaseDataProviderManager
	{
		/// <summary>
		/// 
		/// </summary>
		protected DatabaseSetting Setting
		{
			get;
			private set;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="setting"></param>
		protected BaseDataProviderManager(DatabaseSetting setting)
		{
			if (setting == null)
			{
				throw new ArgumentNullException("setting");
			}
			this.Setting = setting;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public abstract IDataProvider LoadDataProvider();
	}
}
