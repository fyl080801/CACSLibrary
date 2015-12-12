using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace CACSLibrary.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    public class AppDomainTypeFinder : ITypeFinder
    {
        private class AttributedAssembly
        {
            internal Assembly Assembly { get; set; }

            internal Type PluginAttributeType { get; set; }
        }

        private bool loadAppDomainAssemblies = true;
        private string assemblySkipLoadingPattern = "^System|^mscorlib|^Microsoft|^CppCodeProvider|^VJSharpCodeProvider|^WebDev|^Castle|^Iesi|^log4net|^NHibernate|^nunit|^TestDriven|^MbUnit|^Rhino|^QuickGraph|^TestFu|^Telerik|^ComponentArt|^MvcContrib|^AjaxControlToolkit|^Antlr3|^Remotion|^Recaptcha";
        private string assemblyRestrictToLoadingPattern = ".*";
        private IList<string> assemblyNames = new List<string>();
        private readonly List<AppDomainTypeFinder.AttributedAssembly> _attributedAssemblies = new List<AppDomainTypeFinder.AttributedAssembly>();
        private readonly List<Type> _assemblyAttributesSearched = new List<Type>();

        /// <summary>
        /// 
        /// </summary>
        public virtual AppDomain App
        {
            get { return AppDomain.CurrentDomain; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool LoadAppDomainAssemblies
        {
            get { return this.loadAppDomainAssemblies; }
            set { this.loadAppDomainAssemblies = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public IList<string> AssemblyNames
        {
            get { return this.assemblyNames; }
            set { this.assemblyNames = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string AssemblySkipLoadingPattern
        {
            get { return this.assemblySkipLoadingPattern; }
            set { this.assemblySkipLoadingPattern = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string AssemblyRestrictToLoadingPattern
        {
            get { return this.assemblyRestrictToLoadingPattern; }
            set { this.assemblyRestrictToLoadingPattern = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="onlyConcreteClasses"></param>
        /// <returns></returns>
        public IEnumerable<Type> FindClassesOfType<T>(bool onlyConcreteClasses = true)
        {
            return this.FindClassesOfType(typeof(T), onlyConcreteClasses);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assignTypeFrom"></param>
        /// <param name="onlyConcreteClasses"></param>
        /// <returns></returns>
        public IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, bool onlyConcreteClasses = true)
        {
            return this.FindClassesOfType(assignTypeFrom, this.GetAssemblies(), onlyConcreteClasses);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assemblies"></param>
        /// <param name="onlyConcreteClasses"></param>
        /// <returns></returns>
        public IEnumerable<Type> FindClassesOfType<T>(IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true)
        {
            return this.FindClassesOfType(typeof(T), assemblies, onlyConcreteClasses);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assignTypeFrom"></param>
        /// <param name="assemblies"></param>
        /// <param name="onlyConcreteClasses"></param>
        /// <returns></returns>
        public IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true)
        {
            List<Type> list = new List<Type>();
            try
            {
                foreach (Assembly current in assemblies)
                {
                    Type[] types = current.GetTypes();
                    for (int i = 0; i < types.Length; i++)
                    {
                        Type type = types[i];
                        if ((assignTypeFrom.IsAssignableFrom(type) || (assignTypeFrom.IsGenericTypeDefinition && this.DoesTypeImplementOpenGeneric(type, assignTypeFrom))) && !type.IsInterface)
                        {
                            if (onlyConcreteClasses)
                            {
                                if (type.IsClass && !type.IsAbstract)
                                {
                                    list.Add(type);
                                }
                            }
                            else
                            {
                                list.Add(type);
                            }
                        }
                    }
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                string text = string.Empty;
                Exception[] loaderExceptions = ex.LoaderExceptions;
                for (int j = 0; j < loaderExceptions.Length; j++)
                {
                    Exception ex2 = loaderExceptions[j];
                    text = text + ex2.Message + Environment.NewLine;
                }
                Exception ex3 = new Exception(text, ex);
                //Debug.WriteLine(ex3.Message, new object[]
                //{
                //    ex3
                //});
                throw ex3;
            }
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TAssemblyAttribute"></typeparam>
        /// <param name="onlyConcreteClasses"></param>
        /// <returns></returns>
        public IEnumerable<Type> FindClassesOfType<T, TAssemblyAttribute>(bool onlyConcreteClasses = true) where TAssemblyAttribute : Attribute
        {
            IEnumerable<Assembly> assemblies = this.FindAssembliesWithAttribute<TAssemblyAttribute>();
            return this.FindClassesOfType<T>(assemblies, onlyConcreteClasses);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<Assembly> FindAssembliesWithAttribute<T>()
        {
            return this.FindAssembliesWithAttribute<T>(this.GetAssemblies());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public IEnumerable<Assembly> FindAssembliesWithAttribute<T>(IEnumerable<Assembly> assemblies)
        {
            if (!_assemblyAttributesSearched.Contains(typeof(T)))
            {
                var foundAssemblies = (from assembly in assemblies
                                       let customAttributes = assembly.GetCustomAttributes(typeof(T), false)
                                       where customAttributes.Any()
                                       select assembly).ToList();
                _assemblyAttributesSearched.Add(typeof(T));
                foreach (var a in foundAssemblies)
                {
                    _attributedAssemblies.Add(new AttributedAssembly { Assembly = a, PluginAttributeType = typeof(T) });
                }
            }
            return _attributedAssemblies
                .Where(x => x.PluginAttributeType.Equals(typeof(T)))
                .Select(x => x.Assembly)
                .ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assemblyPath"></param>
        /// <returns></returns>
        public IEnumerable<Assembly> FindAssembliesWithAttribute<T>(DirectoryInfo assemblyPath)
        {
            List<Assembly> assemblies = (
                from f in Directory.GetFiles(assemblyPath.FullName, "*.dll")
                select Assembly.LoadFrom(f) into assembly
                let customAttributes = assembly.GetCustomAttributes(typeof(T), false)
                where customAttributes.Any<object>()
                select assembly).ToList<Assembly>();
            return this.FindAssembliesWithAttribute<T>(assemblies);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual IList<Assembly> GetAssemblies()
        {
            List<string> addedAssemblyNames = new List<string>();
            List<Assembly> list = new List<Assembly>();
            if (this.LoadAppDomainAssemblies)
            {
                this.AddAssembliesInAppDomain(addedAssemblyNames, list);
            }
            this.AddConfiguredAssemblies(addedAssemblyNames, list);
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addedAssemblyNames"></param>
        /// <param name="assemblies"></param>
        private void AddAssembliesInAppDomain(List<string> addedAssemblyNames, List<Assembly> assemblies)
        {
            Assembly[] assemblies2 = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies2.Length; i++)
            {
                Assembly assembly = assemblies2[i];
                if (this.Matches(assembly.FullName) && !addedAssemblyNames.Contains(assembly.FullName))
                {
                    assemblies.Add(assembly);
                    addedAssemblyNames.Add(assembly.FullName);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addedAssemblyNames"></param>
        /// <param name="assemblies"></param>
        protected virtual void AddConfiguredAssemblies(List<string> addedAssemblyNames, List<Assembly> assemblies)
        {
            foreach (string current in this.AssemblyNames)
            {
                Assembly assembly = Assembly.Load(current);
                if (!addedAssemblyNames.Contains(assembly.FullName))
                {
                    assemblies.Add(assembly);
                    addedAssemblyNames.Add(assembly.FullName);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyFullName"></param>
        /// <returns></returns>
        public virtual bool Matches(string assemblyFullName)
        {
            return !this.Matches(assemblyFullName, this.AssemblySkipLoadingPattern) && this.Matches(assemblyFullName, this.AssemblyRestrictToLoadingPattern);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyFullName"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        protected virtual bool Matches(string assemblyFullName, string pattern)
        {
            return Regex.IsMatch(assemblyFullName, pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="directoryPath"></param>
        protected virtual void LoadMatchingAssemblies(string directoryPath)
        {
            List<string> list = new List<string>();
            foreach (Assembly current in this.GetAssemblies())
            {
                list.Add(current.FullName);
            }
            if (!Directory.Exists(directoryPath))
            {
                return;
            }
            string[] files = Directory.GetFiles(directoryPath, "*.dll");
            for (int i = 0; i < files.Length; i++)
            {
                string assemblyFile = files[i];
                try
                {
                    AssemblyName assemblyName = AssemblyName.GetAssemblyName(assemblyFile);
                    if (this.Matches(assemblyName.FullName) && !list.Contains(assemblyName.FullName))
                    {
                        this.App.Load(assemblyName);
                    }
                }
                catch (BadImageFormatException ex)
                {
                    Trace.TraceError(ex.ToString());
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="openGeneric"></param>
        /// <returns></returns>
        protected virtual bool DoesTypeImplementOpenGeneric(Type type, Type openGeneric)
        {
            bool result;
            try
            {
                Type genericTypeDefinition = openGeneric.GetGenericTypeDefinition();
                Type[] array = type.FindInterfaces((Type objType, object objCriteria) => true, null);
                for (int i = 0; i < array.Length; i++)
                {
                    Type type2 = array[i];
                    if (type2.IsGenericType)
                    {
                        bool flag = genericTypeDefinition.IsAssignableFrom(type2.GetGenericTypeDefinition());
                        result = flag;
                        return result;
                    }
                }
                result = false;
            }
            catch
            {
                result = false;
            }
            return result;
        }
    }
}
