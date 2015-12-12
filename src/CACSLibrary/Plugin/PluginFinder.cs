using CACSLibrary.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
namespace CACSLibrary.Plugin
{
    /// <summary>
    /// 
    /// </summary>
    public class PluginFinder : IPluginFinder
    {
        IPluginManager _pluginManager;
        IList<PluginDescription> _plugins;
        bool _arePluginsLoaded;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pluginManager"></param>
        public PluginFinder(IPluginManager pluginManager)
        {
            _pluginManager = pluginManager;
        }

        /// <summary>
        /// 
        /// </summary>
        public IPluginManager PluginManager
        {
            get { return _pluginManager; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="installedOnly"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> GetPlugins<T>(bool installedOnly = false) where T : class, IPlugin
        {
            this.EnsurePluginsAreLoaded();
            foreach (PluginDescription current in this._plugins)
            {
                if (typeof(T).IsAssignableFrom(current.PluginType) && (!installedOnly || current.Installed))
                {
                    yield return current.Instance<T>();
                }
            }
            yield break;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="installedOnly"></param>
        /// <returns></returns>
        public virtual IEnumerable<PluginDescription> GetPluginDescriptors(bool installedOnly = false)
        {
            this.EnsurePluginsAreLoaded();
            foreach (PluginDescription current in this._plugins)
            {
                if (!installedOnly || current.Installed)
                {
                    yield return current;
                }
            }
            yield break;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="installedOnly"></param>
        /// <returns></returns>
        public virtual IEnumerable<PluginDescription> GetPluginDescriptors<T>(bool installedOnly = false) where T : class, IPlugin
        {
            this.EnsurePluginsAreLoaded();
            foreach (PluginDescription current in this._plugins)
            {
                if (typeof(T).IsAssignableFrom(current.PluginType) && (!installedOnly || current.Installed))
                {
                    yield return current;
                }
            }
            yield break;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pluginId"></param>
        /// <param name="installedOnly"></param>
        /// <returns></returns>
        public virtual PluginDescription GetPluginDescriptorById(string pluginId, bool installedOnly = false)
        {
            return this.GetPluginDescriptors(installedOnly).SingleOrDefault((PluginDescription p) => p.PluginId.Equals(pluginId));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pluginId"></param>
        /// <param name="installedOnly"></param>
        /// <returns></returns>
        public virtual PluginDescription GetPluginDescriptorById<T>(string pluginId, bool installedOnly = false) where T : class, IPlugin
        {
            return this.GetPluginDescriptors<T>(installedOnly).SingleOrDefault((PluginDescription p) => p.PluginId.Equals(pluginId));
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void ReloadPlugins()
        {
            this._arePluginsLoaded = false;
            this.EnsurePluginsAreLoaded();
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void EnsurePluginsAreLoaded()
        {
            if (!this._arePluginsLoaded)
            {
                this._plugins = this.PluginManager.ReferencedPlugins.ToList();
                this._arePluginsLoaded = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PluginDescription> GetOrderedPlugins()
        {
            var all = this.GetPluginDescriptors(false).ToList();
            IList<PluginDescription> list = new List<PluginDescription>();
            string[] pluginKeys = all.Select(m => m.PluginId).ToArray();
            for (int i = pluginKeys.Length - 1; i >= 0; i--)
            {
                var currentPlugin = this.GetPluginDescriptorById(pluginKeys[i]);
                CheckDependency(currentPlugin, ref list, all);
            }
            return list.ToArray();
        }

        private void CheckDependency(PluginDescription plugin, ref IList<PluginDescription> orderPlugins, IList<PluginDescription> all)
        {
            bool canInstall = true;
            foreach (var dependency in plugin.DependentOn)
            {
                bool haveDependency = all.Any(m => m.PluginId == dependency.PluginId);
                if (!haveDependency)
                {
                    canInstall = false;
                    break;
                }
                PluginDescription dependencyPlugin = all.FirstOrDefault(m => m.PluginId == dependency.PluginId);
                if (dependencyPlugin.Version < dependency.Version)
                {
                    canInstall = false;
                    break;
                }
                CheckDependency(dependencyPlugin, ref orderPlugins, all);
            }
            if (canInstall && !orderPlugins.Any(c => c.PluginId == plugin.PluginId))
            {
                orderPlugins.Add(plugin);
            }
        }
    }
}
