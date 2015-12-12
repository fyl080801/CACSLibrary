using CACSLibrary.Infrastructure;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System.Web.Hosting;

namespace CACSLibrary.Web
{
    public class WebAppTypeFinder : AppDomainTypeFinder
    {
        private bool _binFolderAssembliesLoaded = false;
        private bool _ensureBinFolderAssembliesLoaded = true;
        public WebAppTypeFinder(bool dynamicDiscovery)
        {
            this._ensureBinFolderAssembliesLoaded = dynamicDiscovery;
        }
        public override IList<Assembly> GetAssemblies()
        {
            if (!(!this.EnsureBinFolderAssembliesLoaded || this._binFolderAssembliesLoaded))
            {
                this._binFolderAssembliesLoaded = true;
                string binDirectory = this.GetBinDirectory();
                this.LoadMatchingAssemblies(binDirectory);
            }
            return base.GetAssemblies();
        }
        public virtual string GetBinDirectory()
        {
            if (HostingEnvironment.IsHosted)
            {
                return HttpRuntime.BinDirectory;
            }
            return AppDomain.CurrentDomain.BaseDirectory;
        }
        public bool EnsureBinFolderAssembliesLoaded
        {
            get
            {
                return this._ensureBinFolderAssembliesLoaded;
            }
            set
            {
                this._ensureBinFolderAssembliesLoaded = value;
            }
        }
    }
}

