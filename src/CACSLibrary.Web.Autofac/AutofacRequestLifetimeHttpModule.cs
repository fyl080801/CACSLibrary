using Autofac;
using System;
using System.Web;

namespace CACSLibrary.Web.Autofac
{
    public class AutofacRequestLifetimeHttpModule : IHttpModule
    {
        public static readonly object HttpRequestTag = "AutofacWebRequest";

        private static ILifetimeScope LifetimeScope
        {
            get
            {
                return (ILifetimeScope)HttpContext.Current.Items[typeof(ILifetimeScope)];
            }
            set
            {
                HttpContext.Current.Items[typeof(ILifetimeScope)] = value;
            }
        }

        public void Init(HttpApplication context)
        {
            context.EndRequest += new EventHandler(AutofacRequestLifetimeHttpModule.ContextEndRequest);
        }

        public static ILifetimeScope GetLifetimeScope(ILifetimeScope container, Action<ContainerBuilder> configurationAction)
        {
            if (HttpContext.Current != null)
            {
                return LifetimeScope ?? (LifetimeScope = InitializeLifetimeScope(configurationAction, container));
            }
            else
            {
                return InitializeLifetimeScope(configurationAction, container);
            }
        }

        public void Dispose()
        {
        }

        private static void ContextEndRequest(object sender, EventArgs e)
        {
            ILifetimeScope lifetimeScope = AutofacRequestLifetimeHttpModule.LifetimeScope;
            if (lifetimeScope != null)
            {
                lifetimeScope.Dispose();
            }
        }

        private static ILifetimeScope InitializeLifetimeScope(Action<ContainerBuilder> configurationAction, ILifetimeScope container)
        {
            return (configurationAction == null) ? container.BeginLifetimeScope(AutofacRequestLifetimeHttpModule.HttpRequestTag) : container.BeginLifetimeScope(AutofacRequestLifetimeHttpModule.HttpRequestTag, configurationAction);
        }
    }
}
