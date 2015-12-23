using System;
using System.Collections.Generic;

namespace CACSLibrary.Plugin
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPluginManager
    {
        /// <summary>
        /// 
        /// </summary>
        IEnumerable<PluginDescription> ReferencedPlugins { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pluginId"></param>
        void MakePluginInstalled(string pluginId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pluginId"></param>
        void MakePluginUninstalled(string pluginId);

        /// <summary>
        /// 
        /// </summary>
        void MakeAllPluginUninstalled();
    }
}
