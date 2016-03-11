using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace CACSLibrary.Infrastructure
{
    /// <summary>
    /// 查找类型接口
    /// </summary>
    public interface ITypeFinder
    {
        /// <summary>
        /// 获取运行中的程序集
        /// </summary>
        /// <returns>程序集列表</returns>
        IList<Assembly> GetAssemblies();

        /// <summary>
        /// 获取类型的所有派生类型
        /// </summary>
        /// <param name="assignTypeFrom">类型</param>
        /// <param name="onlyConcreteClasses">只有可实例化的类型</param>
        /// <returns>类型列表</returns>
        IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, bool onlyConcreteClasses = true);

        /// <summary>
        /// 指定程序集，获取类型的所有派生类型
        /// </summary>
        /// <param name="assignTypeFrom">类型</param>
        /// <param name="assemblies">程序集列表</param>
        /// <param name="onlyConcreteClasses">只有可实例化的类型</param>
        /// <returns>类型列表</returns>
        IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true);

        /// <summary>
        /// 根据泛型，获取类型的所有派生类型
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="onlyConcreteClasses">只有可实例化的类型</param>
        /// <returns>类型列表</returns>
        IEnumerable<Type> FindClassesOfType<T>(bool onlyConcreteClasses = true);

        /// <summary>
        /// 根据泛型，获取程序集中的所有类型
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="assemblies">程序集列表</param>
        /// <param name="onlyConcreteClasses">只有可实例化的类型</param>
        /// <returns>类型列表</returns>
        IEnumerable<Type> FindClassesOfType<T>(IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true);

        /// <summary>
        /// 获取包含指定特性的类型
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <typeparam name="TAssemblyAttribute">特性</typeparam>
        /// <param name="onlyConcreteClasses">只有可实例化的类型</param>
        /// <returns>类型列表</returns>
        IEnumerable<Type> FindClassesOfType<T, TAssemblyAttribute>(bool onlyConcreteClasses = true) where TAssemblyAttribute : Attribute;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<Assembly> FindAssembliesWithAttribute<T>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        IEnumerable<Assembly> FindAssembliesWithAttribute<T>(IEnumerable<Assembly> assemblies);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assemblyPath"></param>
        /// <returns></returns>
        IEnumerable<Assembly> FindAssembliesWithAttribute<T>(DirectoryInfo assemblyPath);
    }
}
