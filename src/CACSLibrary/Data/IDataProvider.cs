using System.Data.Common;

namespace CACSLibrary.Data
{
	/// <summary>
	/// ���ݿ⴦��
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
		/// ��ʼ�����ݿ�
		/// </summary>
		void InitDatabase();

		/// <summary>
		/// ��ȡ����
		/// </summary>
		/// <returns></returns>
		DbParameter GetParameter();
	}
}
