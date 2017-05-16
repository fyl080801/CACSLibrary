using System;
using System.Collections.Generic;

namespace CACSLibrary.Infrastructure
{
    /// <summary>
    /// “¿¿µœÓ»›∆˜π‹¿Ì
    /// </summary>
    public interface IContainerManager : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="key"></param>
        /// <param name="lifeStyle"></param>
        void RegisterComponent<TService>(string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="key"></param>
        /// <param name="lifeStyle"></param>
        void RegisterComponent(Type service, string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <param name="key"></param>
        /// <param name="lifeStyle"></param>
        void RegisterComponent<TService, TImplementation>(string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="implementation"></param>
        /// <param name="key"></param>
        /// <param name="lifeStyle"></param>
        void RegisterComponent(Type service, Type implementation, string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="instance"></param>
        /// <param name="key"></param>
        /// <param name="lifeStyle"></param>
        void RegisterComponentInstance<TService>(object instance, string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="instance"></param>
        /// <param name="key"></param>
        /// <param name="lifeStyle"></param>
        void RegisterComponentInstance(Type service, object instance, string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="key"></param>
        /// <param name="lifeStyle"></param>
        void RegisterComponentInstance(object instance, string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="implementation"></param>
        /// <param name="key"></param>
        /// <param name="lifeStyle"></param>
        void RegisterDelegate<TService>(Func<IContainerManager, TService> func, ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <param name="properties"></param>
        /// <param name="key"></param>
        /// <param name="lifeStyle"></param>
        void RegisterComponentWithParameters<TService, TImplementation>(IDictionary<string, string> properties, string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="implementation"></param>
        /// <param name="properties"></param>
        /// <param name="key"></param>
        /// <param name="lifeStyle"></param>
        void RegisterComponentWithParameters(Type service, Type implementation, IDictionary<string, string> properties, string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T Resolve<T>(string key = "") where T : class;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        object Resolve(Type type);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T[] ResolveAll<T>(string key = "");

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T ResolveUnregistered<T>() where T : class;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        object ResolveUnregistered(Type type);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        bool IsRegistered(Type serviceType);

        /// <summary>
        /// 
        /// </summary>
        bool Disposed { get; }
    }
}
