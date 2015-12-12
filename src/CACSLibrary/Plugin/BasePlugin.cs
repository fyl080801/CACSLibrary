using CACSLibrary.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CACSLibrary.Plugin
{
    /// <summary>
    /// ²å¼þ»ù´¡
    /// </summary>
    public abstract class BasePlugin : IPlugin
    {
        /// <summary>
        /// 
        /// </summary>
        protected virtual IPluginManager PluginManager
        {
            get { return EngineContext.Current.Resolve<IPluginManager>(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual PluginDescription Description
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Install()
        {
            this.PluginManager.MakePluginInstalled(this.Description.PluginId);
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Uninstall()
        {
            this.PluginManager.MakePluginUninstalled(this.Description.PluginId);
        }
    }
}
