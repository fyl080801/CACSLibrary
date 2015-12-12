using System;
using System.Collections.Generic;

namespace CACSLibrary.Plugin
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PluginAttribute : Attribute
    {
        string _pluginId;
        string _pluginName;
        string _description;
        IList<Dependency> _dependentOn;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pluginId"></param>
        /// <param name="pluginName"></param>
        public PluginAttribute(string pluginId, string pluginName)
        {
            this._pluginId = pluginId;
            this._pluginName = pluginName;
        }

        /// <summary>
        /// 
        /// </summary>
        public string PluginId
        {
            get { return this._pluginId; }
            set { this._pluginId = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string PluginName
        {
            get { return this._pluginName; }
            set { this._pluginName = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Description
        {
            get { return this._description; }
            set { this._description = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Version Version { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Version SupportedVersion { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IList<Dependency> DependentOn
        {
            get { return _dependentOn ?? (_dependentOn = new List<Dependency>()); }
        }
    }
}
