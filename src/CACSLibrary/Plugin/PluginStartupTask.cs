using CACSLibrary.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CACSLibrary.Plugin
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class PluginStartupTask : IStartupTask
    {
        /// <summary>
        /// 
        /// </summary>
        public abstract EngineLevels Level { get; }

        /// <summary>
        /// 
        /// </summary>
        public abstract string PluginId { get; }

        /// <summary>
        /// 
        /// </summary>
        protected abstract void PluginExecute();

        /// <summary>
        /// 
        /// </summary>
        public void Execute()
        {
            var finder = EngineContext.Current.Resolve<IPluginFinder>();
            var plugin = finder.GetPluginDescriptorById(this.PluginId, false);
            if (plugin.Installed)
                this.PluginExecute();
        }
    }
}
