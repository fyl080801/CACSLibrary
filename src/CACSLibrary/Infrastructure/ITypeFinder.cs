using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace CACSLibrary.Infrastructure
{
    /// <summary>
    /// �������ͽӿ�
    /// </summary>
    public interface ITypeFinder
    {
        /// <summary>
        /// ��ȡ�����еĳ���
        /// </summary>
        /// <returns>�����б�</returns>
        IList<Assembly> GetAssemblies();

        /// <summary>
        /// ��ȡ���͵�������������
        /// </summary>
        /// <param name="assignTypeFrom">����</param>
        /// <param name="onlyConcreteClasses">ֻ�п�ʵ����������</param>
        /// <returns>�����б�</returns>
        IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, bool onlyConcreteClasses = true);

        /// <summary>
        /// ָ�����򼯣���ȡ���͵�������������
        /// </summary>
        /// <param name="assignTypeFrom">����</param>
        /// <param name="assemblies">�����б�</param>
        /// <param name="onlyConcreteClasses">ֻ�п�ʵ����������</param>
        /// <returns>�����б�</returns>
        IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true);

        /// <summary>
        /// ���ݷ��ͣ���ȡ���͵�������������
        /// </summary>
        /// <typeparam name="T">����</typeparam>
        /// <param name="onlyConcreteClasses">ֻ�п�ʵ����������</param>
        /// <returns>�����б�</returns>
        IEnumerable<Type> FindClassesOfType<T>(bool onlyConcreteClasses = true);

        /// <summary>
        /// ���ݷ��ͣ���ȡ�����е���������
        /// </summary>
        /// <typeparam name="T">����</typeparam>
        /// <param name="assemblies">�����б�</param>
        /// <param name="onlyConcreteClasses">ֻ�п�ʵ����������</param>
        /// <returns>�����б�</returns>
        IEnumerable<Type> FindClassesOfType<T>(IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true);

        /// <summary>
        /// ��ȡ����ָ�����Ե�����
        /// </summary>
        /// <typeparam name="T">����</typeparam>
        /// <typeparam name="TAssemblyAttribute">����</typeparam>
        /// <param name="onlyConcreteClasses">ֻ�п�ʵ����������</param>
        /// <returns>�����б�</returns>
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
