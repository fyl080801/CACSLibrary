using System.Data.Common;

namespace CACSLibrary.Data
{
	/// <summary>
	/// 数据库处理
	/// </summary>
	public interface IDataProvider
	{
		/// <summary>
		/// 
		/// </summary>
		bool StoredProceduredSupported
		{
			get;
		}

		/// <summary>
		/// 初始化数据库
		/// </summary>
		void InitDatabase();

		/// <summary>
		/// 获取参数
		/// </summary>
		/// <returns></returns>
		DbParameter GetParameter();
	}
}
