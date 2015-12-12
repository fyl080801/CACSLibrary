using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace CACSLibrary.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class InjectionConfig : ConfigurationSection
    {
        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("loaderType")]
        public string loaderType
        {
            get
            {
                return base["loaderType"] as string;
            }
        }
    }
}
