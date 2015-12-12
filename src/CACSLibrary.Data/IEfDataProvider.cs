using System;
using System.Data.Entity.Infrastructure;

namespace CACSLibrary.Data
{
    /// <summary>
    /// 
    /// </summary>
	public interface IEfDataProvider : IDataProvider
	{
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		IDbConnectionFactory GetConnectionFactory();

        /// <summary>
        /// 
        /// </summary>
		void InitConnectionFactory();

        /// <summary>
        /// 
        /// </summary>
		void SetDatabaseInitializer();
	}
}
