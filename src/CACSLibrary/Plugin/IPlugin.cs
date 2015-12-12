using System;
using System.Collections.Generic;

namespace CACSLibrary.Plugin
{
    /// <summary>
    /// 插件接口
    /// </summary>
    /// <remarks>
    /// 此类是所有插件的基础接口，如果要在程序中定义插件，必须包含这个接口
    /// </remarks>
    public interface IPlugin
    {
        /// <summary>
        /// 插件描述信息
        /// </summary>
        PluginDescription Description { get; set; }

        /// <summary>
        /// 安装
        /// </summary>
        void Install();

        /// <summary>
        /// 卸载
        /// </summary>
        void Uninstall();
    }
}
