using Autofac;
using Autofac.Builder;
using CACSLibrary.Infrastructure;
using CACSLibrary.Interceptor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CACSLibrary.Autofac
{
    public class ContainerManager : IContainerManager
    {
        private ContainerBuilder _builder = new ContainerBuilder();
        private readonly IContainer _container;

        public ContainerManager()
        {
            this._container = this._builder.Build(ContainerBuildOptions.None);
        }

        public bool IsRegistered(Type serviceType)
        {
            return this.Scope().IsRegistered(serviceType);
        }

        public void RegisterComponent<TService>(string key = "", ComponentLifeStyle lifeStyle = 0)
        {
            this.RegisterComponent(typeof(TService), key, lifeStyle);
        }

        public void RegisterComponent<TService, TImplementation>(string key = "", ComponentLifeStyle lifeStyle = 0)
        {
            this.RegisterComponent(typeof(TService), typeof(TImplementation), key, lifeStyle);
        }

        public void RegisterComponent(Type service, string key = "", ComponentLifeStyle lifeStyle = 0)
        {
            this.RegisterComponent(service, service, key, lifeStyle);
        }

        public void RegisterComponent(Type service, Type implementation, string key = "", ComponentLifeStyle lifeStyle = 0)
        {
            UpdateContainer(x =>
            {
                var serviceTypes = new List<Type> { service };

                if (service.IsGenericType)
                {
                    var temp = x.RegisterGeneric(implementation).As(
                        serviceTypes.ToArray());//.PerLifeStyle(lifeStyle);
                    temp = this.PerLifeStyle<object, ReflectionActivatorData, DynamicRegistrationStyle>(temp, lifeStyle);
                    if (!string.IsNullOrEmpty(key))
                    {
                        temp.Keyed(key, service);
                    }
                }
                else
                {
                    var temp = x.RegisterType(implementation).As(
                        serviceTypes.ToArray());//.PerLifeStyle(lifeStyle);
                    temp = this.PerLifeStyle<object, ConcreteReflectionActivatorData, SingleRegistrationStyle>(temp, lifeStyle);
                    if (!string.IsNullOrEmpty(key))
                    {
                        temp.Keyed(key, service);
                    }
                }
            });
        }

        public void RegisterComponentInstance(object instance, string key = "", ComponentLifeStyle lifeStyle = 0)
        {
            this.RegisterComponentInstance(instance.GetType(), instance, key, lifeStyle);
        }

        public void RegisterComponentInstance<TService>(object instance, string key = "", ComponentLifeStyle lifeStyle = 0)
        {
            this.RegisterComponentInstance(typeof(TService), instance, key, lifeStyle);
        }

        public void RegisterComponentInstance(Type service, object instance, string key = "", ComponentLifeStyle lifeStyle = 0)
        {
            UpdateContainer(x =>
            {
                var registration = x.RegisterInstance(instance).Keyed(key, service).As(service);//.PerLifeStyle(lifeStyle);
                registration = this.PerLifeStyle<object, SimpleActivatorData, SingleRegistrationStyle>(registration, lifeStyle);
            });
        }

        public void RegisterComponentWithParameters<TService, TImplementation>(IDictionary<string, string> properties, string key = "", ComponentLifeStyle lifeStyle = 0)
        {
            this.RegisterComponentWithParameters(typeof(TService), typeof(TImplementation), properties, "", ComponentLifeStyle.Singleton);
        }

        public void RegisterComponentWithParameters(Type service, Type implementation, IDictionary<string, string> properties, string key = "", ComponentLifeStyle lifeStyle = 0)
        {
            UpdateContainer(x =>
            {
                var serviceTypes = new List<Type> { service };
                var temp = x.RegisterType(implementation).As(serviceTypes.ToArray())
                    .WithParameters(properties.Select(y => new global::Autofac.Core.NamedPropertyParameter(y.Key, y.Value)).ToArray());
                //#if net35
                //                var temp = x.RegisterType(implementation).As(serviceTypes.ToArray())
                //                    .WithParameters(properties.Select(y => new global::Autofac.Core.NamedPropertyParameter(y.Key, y.Value)).ToArray());
                //#else
                //                var temp = x.RegisterType(implementation).As(serviceTypes.ToArray())
                //                   .WithParameters(properties.Select(y => new NamedParameter(y.Key, y.Value)));
                //#endif
                if (!string.IsNullOrEmpty(key))
                {
                    temp.Keyed(key, service);
                }
            });
        }

        public T Resolve<T>(string key = "") where T : class
        {
            T obj = string.IsNullOrEmpty(key) ?
                this.Scope().Resolve<T>() :
                this.Scope().ResolveKeyed<T>(key);
            if (obj is MarshalByRefObject)
                return Injection.Wrap<T>(obj);
            else
                return obj;
        }

        public object Resolve(Type type)
        {
            var obj = this.Scope().Resolve(type);
            if (obj is MarshalByRefObject)
                return Injection.Wrap(type, obj);
            else
                return obj;
        }

        public T[] ResolveAll<T>(string key = "")
        {
            T[] list = string.IsNullOrEmpty(key) ?
                this.Scope().Resolve<IEnumerable<T>>().ToArray<T>() :
                this.Scope().ResolveKeyed<IEnumerable<T>>(key).ToArray<T>();
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i] is MarshalByRefObject)
                    list[i] = Injection.Wrap<T>(list[i]);
            }
            return list;
        }

        public T ResolveUnregistered<T>() where T : class
        {
            return this.ResolveUnregistered(typeof(T)) as T;
        }

        public object ResolveUnregistered(Type type)
        {
            ConstructorInfo[] constructors = type.GetConstructors();
            foreach (ConstructorInfo constructor in constructors)
            {
                try
                {
                    ParameterInfo[] parameters = constructor.GetParameters();
                    List<object> list = new List<object>();
                    foreach (ParameterInfo param in parameters)
                    {
                        object item = this.Resolve(param.ParameterType);
                        if (item == null)
                        {
                            throw new CACSException(string.Format("未知的类型 {0}", type.Name));
                        }
                        list.Add(item);
                    }
                    var obj = Activator.CreateInstance(type, list.ToArray());
                    if (obj is MarshalByRefObject)
                        return Injection.Wrap(type, obj);
                    else
                        return obj;
                }
                catch (CACSException ex)
                {
                    throw ex;
                }
            }
            throw new CACSException(string.Format("找不到类型 {0} 的构造函数", type.Name));
        }

        public virtual ILifetimeScope Scope()
        {
            return this.Container;
        }

        public void UpdateContainer(Action<ContainerBuilder> action)
        {
            ContainerBuilder builder = new ContainerBuilder();
            action(builder);
            builder.Update(this._container);
        }

        public IContainer Container
        {
            get
            {
                return this._container;
            }
        }

        protected virtual IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> PerLifeStyle<TLimit, TActivatorData, TRegistrationStyle>(IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> builder, ComponentLifeStyle lifeStyle)
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
                    result = builder.InstancePerLifetimeScope();
                    break;
                default:
                    result = builder.SingleInstance();
                    break;
            }
            return result;
        }
    }

    public static class ContainerManagerExtensions
    {
        public static void RegisterAsDelegate<T>(this IContainerManager containerManager, Action<ContainerBuilder> action)
        {
            ((ContainerManager)containerManager).UpdateContainer(action);
        }
    }
}