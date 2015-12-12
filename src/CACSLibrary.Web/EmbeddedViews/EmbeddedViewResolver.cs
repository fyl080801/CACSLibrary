using CACSLibrary.Infrastructure;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CACSLibrary.Web.EmbeddedViews
{
    public class EmbeddedViewResolver : IEmbeddedViewResolver
    {
        private ITypeFinder _typeFinder;

        public EmbeddedViewResolver(ITypeFinder typeFinder)
        {
            this._typeFinder = typeFinder;
        }

        public EmbeddedViewTable GetEmbeddedViews()
        {
            IList<Assembly> assemblies = this._typeFinder.GetAssemblies();
            if ((assemblies == null) || (assemblies.Count == 0))
            {
                return null;
            }
            EmbeddedViewTable table = new EmbeddedViewTable();
            foreach (Assembly assembly in assemblies)
            {
                string[] namesOfAssemblyResources = GetNamesOfAssemblyResources(assembly);
                if ((namesOfAssemblyResources != null) && (namesOfAssemblyResources.Length != 0))
                {
                    foreach (string str in namesOfAssemblyResources)
                    {
                        if (str.ToLowerInvariant().Contains(".views."))
                        {
                            table.AddView(str, assembly.FullName);
                        }
                    }
                }
            }
            return table;
        }

        private static string[] GetNamesOfAssemblyResources(Assembly assembly)
        {
            try
            {
                return assembly.GetManifestResourceNames();
            }
            catch
            {
                return new string[0];
            }
        }
    }
}

