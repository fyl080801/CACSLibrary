using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CACSLibrary.Plugin
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class BasePluginManager : IPluginManager
    {
        IEnumerable<PluginDescription> _referencedPlugins;

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<PluginDescription> ReferencedPlugins
        {
            get { return _referencedPlugins ?? (_referencedPlugins = new List<PluginDescription>()); }
            set { _referencedPlugins = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pluginId"></param>
        public void MakePluginInstalled(string pluginId)
        {
            var plugins = this.ReferencedPlugins;
            if (!this._referencedPlugins.Any(m => m.PluginId == pluginId))
                throw new PluginException(pluginId, (int)PluginErrors.Other, "未找到插件");

            var installPlugin = this._referencedPlugins.FirstOrDefault(m => m.PluginId == pluginId);
            IEnumerable<PluginDescription> source =
                from m in plugins
                where m.Installed
                select m;
            StringBuilder messageBuilder = new StringBuilder("");
            using (IEnumerator<Dependency> enumerator = installPlugin.DependentOn.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Dependency dependent = enumerator.Current;
                    if (source.Count((PluginDescription m) => m.PluginId == dependent.PluginId && m.Version >= dependent.Version) <= 0)
                    {
                        messageBuilder.AppendLine(string.Format("{0} v{1}", dependent.PluginId, dependent.Version.ToString()));
                    }
                }
            }
            if (messageBuilder.Length <= 0)
            {
                this.PluginInstall(pluginId);
                return;
            }
            StringBuilder outputBuilder = new StringBuilder("以下依赖项未安装:");
            outputBuilder.AppendLine(messageBuilder.ToString());
            throw new PluginException(installPlugin.PluginId, 1, outputBuilder.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pluginId"></param>
        public void MakePluginUninstalled(string pluginId)
        {
            var plugins = this.ReferencedPlugins;
            if (!this._referencedPlugins.Any(m => m.PluginId == pluginId))
                throw new PluginException(pluginId, (int)PluginErrors.Other, "未找到插件");

            var installPlugin = this._referencedPlugins.FirstOrDefault(m => m.PluginId == pluginId);
            IEnumerable<PluginDescription> enumerable =
                from m in plugins
                where m.Installed
                select m;
            StringBuilder messageBuilder = new StringBuilder("");
            foreach (PluginDescription current in enumerable)
            {
                if (current.DependentOn.Count((Dependency m) => m.PluginId == installPlugin.PluginId && m.Version <= installPlugin.Version) > 0)
                {
                    messageBuilder.AppendLine(string.Format("{0} v{1}", current.PluginId, current.Version.ToString()));
                }
            }
            if (messageBuilder.Length <= 0)
            {
                this.PluginUninstall(pluginId);
                return;
            }
            StringBuilder outputBuilder = new StringBuilder("以下插件需要此依赖项:");
            outputBuilder.AppendLine(messageBuilder.ToString());
            throw new PluginException(installPlugin.PluginId, 2, outputBuilder.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        public void MakeAllPluginUninstalled()
        {
            this.AllPluginUninstall();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pluginId"></param>
        protected abstract void PluginInstall(string pluginId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pluginId"></param>
        protected abstract void PluginUninstall(string pluginId);

        /// <summary>
        /// 
        /// </summary>
        protected abstract void AllPluginUninstall();
    }
}
