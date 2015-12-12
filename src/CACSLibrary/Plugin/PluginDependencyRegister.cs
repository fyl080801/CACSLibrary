using CACSLibrary.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CACSLibrary.Plugin
{
    /// <summary>
    /// 插件依赖项注册
    /// </summary>
    /// <remarks>
    /// 使用这个类作为依赖项注册的插件，在没有安装时不会执行注册
    /// </remarks>
    public abstract class PluginDependencyRegister : IDependencyRegister
    {
        /// <summary>
        /// 插件Id
        /// </summary>
        public abstract string PluginId { get; }

        /// <summary>
        /// 插件注册依赖项
        /// </summary>
        /// <param name="containerManager">容器</param>
        /// <param name="typeFinder">类型查找</param>
        protected abstract void PluginRegister(IContainerManager containerManager, ITypeFinder typeFinder);

        /// <summary>
        /// 级别
        /// </summary>
        /// <remarks>
        /// <note type="caution">
        /// 插件中最好不要用 Priority 值
        /// </note>
        /// </remarks>
        public abstract EngineLevels Level { get; }

        /// <summary>
        /// 注册依赖项
        /// </summary>
        /// <param name="containerManager">容器</param>
        /// <param name="typeFinder">类型查找</param>
        public void Register(IContainerManager containerManager, ITypeFinder typeFinder)
        {
            var finder = EngineContext.Current.Resolve<IPluginFinder>(); ;
            var plugin = finder.GetPluginDescriptorById(this.PluginId, false);
            if (plugin.Installed)
                this.PluginRegister(containerManager, typeFinder);
        }
    }
}
