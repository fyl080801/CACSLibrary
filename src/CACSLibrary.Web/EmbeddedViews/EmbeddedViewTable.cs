using System;
using System.Collections.Generic;
using System.Linq;

namespace CACSLibrary.Web.EmbeddedViews
{
    public class EmbeddedViewTable
    {
        private static readonly object _lockHelper = new object();
        private readonly Dictionary<string, EmbeddedViewMetadata> _viewCache = new Dictionary<string, EmbeddedViewMetadata>();

        public void AddView(string viewName, string assemblyName)
        {
            lock (_lockHelper)
            {
                EmbeddedViewMetadata metadata = new EmbeddedViewMetadata {
                    Name = viewName,
                    AssemblyFullName = assemblyName
                };
                this._viewCache[viewName] = metadata;
            }
        }

        public bool ContainsEmbeddedView(string viewName)
        {
            return (this.FindEmbeddedView(viewName) != null);
        }

        public EmbeddedViewMetadata FindEmbeddedView(string viewName)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                return null;
            }
            return (from view in this.Views
                where view.Name.ToLowerInvariant().Equals(viewName.ToLowerInvariant())
                select view).SingleOrDefault<EmbeddedViewMetadata>();
        }

        protected string GetNameFromPath(string viewPath)
        {
            if (string.IsNullOrEmpty(viewPath))
            {
                return null;
            }
            return viewPath.Replace("/", ".").Replace("~", "");
        }

        public IList<EmbeddedViewMetadata> Views
        {
            get
            {
                return this._viewCache.Values.ToList<EmbeddedViewMetadata>();
            }
        }
    }
}

