using System;
using System.Collections.Generic;

namespace CACSLibrary.Plugin
{
    /// <summary>
    /// 查找插件
    /// </summary>
    public interface IPluginFinder
    {
        /// <summary>
        /// 插件管理
        /// </summary>
        IPluginManager PluginManager { get; }

        /// <summary>
        /// 获取所有插件
        /// </summary>
        /// <typeparam name="T">插件类型</typeparam>
        /// <param name="installedOnly">只查找已安装的</param>
        /// <returns>插件列表</returns>
        IEnumerable<T> GetPlugins<T>(bool installedOnly = false) where T : class, IPlugin;

        /// <summary>
        /// 获取所有插件描述文件
        /// </summary>
        /// <param name="installedOnly">只查找已安装的</param>
        /// <returns>插件描述列表</returns>
        IEnumerable<PluginDescription> GetPluginDescriptors(bool installedOnly = false);

        /// <summary>
        /// 获得按照依赖关系排序后的插件描述文件
        /// </summary>
        /// <returns>插件描述列表</returns>
        IEnumerable<PluginDescription> GetOrderedPlugins();

        /// <summary>
        /// 获取指定类型的插件的描述文件
        /// </summary>
        /// <typeparam name="T">插件类型</typeparam>
        /// <param name="installedOnly">只查找已安装的</param>
        /// <returns>插件描述列表</returns>
        IEnumerable<PluginDescription> GetPluginDescriptors<T>(bool installedOnly = false) where T : class, IPlugin;

        /// <summary>
        /// 通过Id获取插件描述文件
        /// </summary>
        /// <param name="pluginId">插件Id</param>
        /// <param name="installedOnly">只查找已安装的</param>
        /// <returns>插件描述文件</returns>
        PluginDescription GetPluginDescriptorById(string pluginId, bool installedOnly = false);

        /// <summary>
        /// 通过Id获取指定类型的插件描述文件
        /// </summary>
        /// <typeparam name="T">插件类型</typeparam>
        /// <param name="pluginId">插件Id</param>
        /// <param name="installedOnly">只查找已安装的</param>
        /// <returns>插件描述文件</returns>
        PluginDescription GetPluginDescriptorById<T>(string pluginId, bool installedOnly = false) where T : class, IPlugin;

        /// <summary>
        /// 重新入去插件
        /// </summary>
        void ReloadPlugins();
    }
}
