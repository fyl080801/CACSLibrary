using System;
using System.Collections;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;

namespace CACSLibrary.Web.EmbeddedViews
{
    public class EmbeddedViewVirtualPathProvider : VirtualPathProvider
    {
        private readonly EmbeddedViewTable _embeddedViews;

        public EmbeddedViewVirtualPathProvider(EmbeddedViewTable embeddedViews)
        {
            if (embeddedViews == null)
            {
                throw new ArgumentNullException("embeddedViews");
            }
            this._embeddedViews = embeddedViews;
        }

        public override bool FileExists(string virtualPath)
        {
            return (this.IsEmbeddedView(virtualPath) || base.Previous.FileExists(virtualPath));
        }

        public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            return (this.IsEmbeddedView(virtualPath) ? null : base.Previous.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart));
        }

        public override VirtualFile GetFile(string virtualPath)
        {
            if (this.IsEmbeddedView(virtualPath))
            {
                string str = VirtualPathUtility.ToAppRelative(virtualPath);
                string viewName = str.Substring(str.LastIndexOf("/") + 1, (str.Length - 1) - str.LastIndexOf("/"));
                return new EmbeddedResourceVirtualFile(this._embeddedViews.FindEmbeddedView(viewName), virtualPath);
            }
            return base.Previous.GetFile(virtualPath);
        }

        private bool IsEmbeddedView(string virtualPath)
        {
            if (string.IsNullOrEmpty(virtualPath))
            {
                return false;
            }
            string str = VirtualPathUtility.ToAppRelative(virtualPath);
            if (!str.StartsWith("~/Views/", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }
            string viewName = str.Substring(str.LastIndexOf("/") + 1, (str.Length - 1) - str.LastIndexOf("/"));
            return this._embeddedViews.ContainsEmbeddedView(viewName);
        }
    }
}

