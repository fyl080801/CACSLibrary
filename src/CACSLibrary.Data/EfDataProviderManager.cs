using System;

namespace CACSLibrary.Data
{
	/// <summary>
	/// 
	/// </summary>
	public class EfDataProviderManager : BaseDataProviderManager
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="config"></param>
		public EfDataProviderManager(DatabaseSetting config) : base(config)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override IDataProvider LoadDataProvider()
		{
			string providerName = base.Setting.DataProvider;
			IDataProvider result;
			if (!string.IsNullOrWhiteSpace(providerName))
			{
				string text = providerName.ToLowerInvariant();
				if (text != null)
				{
					if (text == "sqlserver")
					{
						result = new SqlServerDataProvider();
						return result;
					}
				}
				throw new Exception(string.Format("不支持的 dataprovider: {0}", providerName));
			}
			result = new SqlServerDataProvider();
			return result;
		}
	}
}
