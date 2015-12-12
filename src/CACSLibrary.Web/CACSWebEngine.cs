using CACSLibrary;
using CACSLibrary.Configuration;
using CACSLibrary.Infrastructure;
using CACSLibrary.Plugin;
using CACSLibrary.Web.Plugin;
using System;
using System.Configuration;

namespace CACSLibrary.Web
{
    public class CACSWebEngine : CACSEngine
    {
        protected override void RegisterTypeFinder()
        {
            bool dynamicDiscovery = ConfigurationManager.AppSettings["dynamicDiscovery"].ToBoolean(true);
            base.ContainerManager.RegisterComponentInstance<ITypeFinder>(
                new WebAppTypeFinder(dynamicDiscovery),
                typeof(WebAppTypeFinder).FullName,
                ComponentLifeStyle.Singleton);
        }

        protected override IContainerManager InitializeContainer()
        {
            EngineConfig engineConfig = ConfigurationManager.GetSection("engineConfig") as EngineConfig;
            if (engineConfig == null || engineConfig.ContainerType == null || string.IsNullOrEmpty(engineConfig.ContainerType))
            {
                return (IContainerManager)Activator.CreateInstance(Type.GetType("CACSLibrary.Web.Autofac.WebContainerManager, CACSLibrary.Web.Autofac"));
            }
            Type containerType = Type.GetType(engineConfig.ContainerType);
            if (containerType == null)
            {
                throw new ConfigurationErrorsException("ContainerManager 为空");
            }
            return (IContainerManager)Activator.CreateInstance(containerType);
        }

        //protected override void RegisterPlugin(CACSConfig configuration)
        //{
        //    if (!Singleton<IPluginManager>.IsInstanceNull)
        //        base.ContainerManager.RegisterComponentInstance<IPluginManager>(Singleton<IPluginManager>.Instance);
        //    else
        //        base.ContainerManager.RegisterComponentInstance<IPluginManager>(new WebPluginManager());
        //}
    }
}

