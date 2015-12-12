using System;
using System.Runtime.CompilerServices;

namespace CACSLibrary.Web.EmbeddedViews
{
    [Serializable]
    public class EmbeddedViewMetadata
    {
        public string AssemblyFullName { get; set; }

        public string Name { get; set; }
    }
}

