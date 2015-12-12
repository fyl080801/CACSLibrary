using System;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace CACSLibrary.Data
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class BaseEfDataProvider : IEfDataProvider, IDataProvider
    {
        /// <summary>
        /// 
        /// </summary>
        public abstract bool StoredProceduredSupported { get; }

        /// <summary>
        /// 
        /// </summary>
        public void InitConnectionFactory()
        {
#pragma warning disable 0618
            Database.DefaultConnectionFactory = this.GetConnectionFactory();
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void InitDatabase()
        {
            this.InitConnectionFactory();
            this.SetDatabaseInitializer();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract IDbConnectionFactory GetConnectionFactory();

        /// <summary>
        /// 
        /// </summary>
        public abstract void SetDatabaseInitializer();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract DbParameter GetParameter();
    }
}
