using Autofac;
using Autofac.Builder;
using CACSLibrary.Autofac;
using CACSLibrary.Infrastructure;
using System.Web;

namespace CACSLibrary.Web.Autofac
{
    public class WebContainerManager : ContainerManager
    {
        public override ILifetimeScope Scope()
        {
            try
            {
                return AutofacRequestLifetimeHttpModule.GetLifetimeScope(this.Container, null);
            }
            catch
            {
                return base.Container;
            }
        }

        protected override IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> PerLifeStyle<TLimit, TActivatorData, TRegistrationStyle>(IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> builder, Infrastructure.ComponentLifeStyle lifeStyle)
        {
            IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> result;
            switch (lifeStyle)
            {
                case ComponentLifeStyle.Singleton:
                    result = builder.SingleInstance();
                    break;
                case ComponentLifeStyle.Transient:
                    result = builder.InstancePerDependency();
                    break;
                case ComponentLifeStyle.LifetimeScope:
                    result = ((HttpContext.Current != null) ? builder.InstancePerRequest() : builder.InstancePerLifetimeScope());
                    break;
                default:
                    result = builder.SingleInstance();
                    break;
            }
            return result;
        }
    }
}
