using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Hosting;

namespace CACSLibrary.Web.EmbeddedViews
{
    public class EmbeddedResourceVirtualFile : VirtualFile
    {
        private readonly EmbeddedViewMetadata _metadata;

        public EmbeddedResourceVirtualFile(EmbeddedViewMetadata metadata, string virtualPath) : base(virtualPath)
        {
            if (metadata == null)
            {
                throw new ArgumentNullException("metadata");
            }
            this._metadata = metadata;
        }

        public override Stream Open()
        {
            Assembly assembly = (from m in AppDomain.CurrentDomain.GetAssemblies()
                where string.Equals(m.FullName, this._metadata.AssemblyFullName)
                select m).FirstOrDefault<Assembly>();
            if (assembly != null)
            {
                return assembly.GetManifestResourceStream(this._metadata.Name);
            }
            return null;
        }
    }
}

