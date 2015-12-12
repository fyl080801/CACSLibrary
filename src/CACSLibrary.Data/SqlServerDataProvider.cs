using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace CACSLibrary.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class SqlServerDataProvider : BaseEfDataProvider
    {
        /// <summary>
        /// 
        /// </summary>
        public override bool StoredProceduredSupported
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IDbConnectionFactory GetConnectionFactory()
        {
            return new SqlConnectionFactory();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void SetDatabaseInitializer()
        {
            Database.SetInitializer<CACSObjectContext>(new CreateTablesIfNotExist<CACSObjectContext>(new string[0]));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="throwExceptionIfNonExists"></param>
        /// <returns></returns>
        protected virtual string[] ParseCommands(string filePath, bool throwExceptionIfNonExists)
        {
            string[] result;
            if (!File.Exists(filePath))
            {
                if (throwExceptionIfNonExists)
                {
                    throw new ArgumentException(string.Format("Specified 文件不存在 - {0}", filePath));
                }
                result = new string[0];
            }
            else
            {
                List<string> statements = new List<string>();
                using (FileStream stream = File.OpenRead(filePath))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string statement;
                        while ((statement = this.readNextStatementFromStream(reader)) != null)
                        {
                            statements.Add(statement);
                        }
                    }
                }
                result = statements.ToArray();
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected virtual string readNextStatementFromStream(StreamReader reader)
        {
            StringBuilder sb = new StringBuilder();
            string result;
            while (true)
            {
                string lineOfText = reader.ReadLine();
                if (lineOfText == null)
                {
                    break;
                }
                if (lineOfText.TrimEnd(new char[0]).ToUpper() == "GO")
                {
                    result = sb.ToString();
                    return result;
                }
                sb.Append(lineOfText + Environment.NewLine);
            }
            if (sb.Length > 0)
            {
                result = sb.ToString();
                return result;
            }
            result = null;
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override DbParameter GetParameter()
        {
            return new SqlParameter();
        }
    }
}
