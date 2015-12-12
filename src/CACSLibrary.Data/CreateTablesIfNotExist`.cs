using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Transactions;

namespace CACSLibrary.Data
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class CreateTablesIfNotExist<TContext> : IDatabaseInitializer<TContext> where TContext : DbContext
    {
        private readonly string[] _customCommands;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customCommands"></param>
        public CreateTablesIfNotExist(string[] customCommands)
        {
            this._customCommands = customCommands;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void InitializeDatabase(TContext context)
        {
            bool dbExists;
            using (new TransactionScope(TransactionScopeOption.Suppress))
            {
                dbExists = context.Database.Exists();
            }
            if (!dbExists)
            {
                context.Database.CreateIfNotExists();
            }
            int numberOfTables = 0;
            foreach (int t in context.Database.SqlQuery<int>("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE table_type = 'BASE TABLE' ", new object[0]))
            {
                numberOfTables = t;
            }
            bool createTables = numberOfTables == 0;
            if (createTables)
            {
                string dbCreationScript = ((IObjectContextAdapter)context).ObjectContext.CreateDatabaseScript();
                context.Database.ExecuteSqlCommand(dbCreationScript, new object[0]);
                context.SaveChanges();
                if (this._customCommands != null && this._customCommands.Length > 0)
                {
                    string[] customCommands = this._customCommands;
                    for (int i = 0; i < customCommands.Length; i++)
                    {
                        string command = customCommands[i];
                        context.Database.ExecuteSqlCommand(command, new object[0]);
                    }
                }
            }
        }
    }
}
