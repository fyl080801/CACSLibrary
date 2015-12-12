using System.Collections.Generic;

namespace CACSLibrary.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class DatabaseSetting
    {
        /// <summary>
        /// 
        /// </summary>
        public string DataProvider { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> EntityMapAssmbly { get; set; }

        //public bool OneToManyCollectionWrapperEnabled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsInstalled
        {
            get { return !string.IsNullOrEmpty(this.ConnectionString); }
        }
    }
}
