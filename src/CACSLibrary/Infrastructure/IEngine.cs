using CACSLibrary.Configuration;
using System;

namespace CACSLibrary.Infrastructure
{
    /// <summary>
    /// 引擎
    /// </summary>
    /// <remarks>引擎管理了程序运行的基本行为，包括对象容器和对象容器取出相关类型的实例</remarks>
    public interface IEngine
    {
        /// <summary>
        /// 对象容器
        /// </summary>
        IContainerManager ContainerManager
        {
            get;
        }

        /// <summary>
        /// 初始化引擎
        /// </summary>
        /// <param name="config">配置</param>
        void Initialize(CACSConfig config);

        /// <summary>
        /// 返回指定类型的实例
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>实例</returns>
        T Resolve<T>() where T : class;

        /// <summary>
        /// 返回指定类型的实例
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>实例</returns>
        object Resolve(Type type);

        /// <summary>
        /// 返回类型的所有实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T[] ResolveAll<T>();
    }
}
