using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace CACSLibrary.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
	public interface ITypeFinder
	{
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		IList<Assembly> GetAssemblies();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assignTypeFrom"></param>
        /// <param name="onlyConcreteClasses"></param>
        /// <returns></returns>
		IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, bool onlyConcreteClasses = true);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assignTypeFrom"></param>
        /// <param name="assemblies"></param>
        /// <param name="onlyConcreteClasses"></param>
        /// <returns></returns>
		IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="onlyConcreteClasses"></param>
        /// <returns></returns>
		IEnumerable<Type> FindClassesOfType<T>(bool onlyConcreteClasses = true);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assemblies"></param>
        /// <param name="onlyConcreteClasses"></param>
        /// <returns></returns>
		IEnumerable<Type> FindClassesOfType<T>(IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TAssemblyAttribute"></typeparam>
        /// <param name="onlyConcreteClasses"></param>
        /// <returns></returns>
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
