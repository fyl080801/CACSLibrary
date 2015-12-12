using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CACSLibrary.Data
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class MigrationsConfiguration : DbMigrationsConfiguration<CACSObjectContext>
    {
        /// <summary>
        /// 
        /// </summary>
        public MigrationsConfiguration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        protected override void Seed(CACSObjectContext context)
        {
            
        }
    }
}
