using CACSLibrary.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace CACSLibrary.Plugin
{
    /// <summary>
    /// 
    /// </summary>
    public class PluginDescription : IComparable<PluginDescription>
    {
        IList<Dependency> _dependentOn;

        IList<string> _tags;

        /// <summary>
        /// 
        /// </summary>
        public virtual Assembly PluginAssembly { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual Type PluginType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual FileInfo PluginFile { get; set; }

        /// <summary>
        /// 插件所在物理路径
        /// </summary>
        public virtual string PluginPath
        {
            get { return this.PluginFile.DirectoryName; }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual bool Installed { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string PluginFileName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string PluginId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string PluginName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Remark { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual int Index { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual Version Version { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual Version SupportedVersion { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual IList<Dependency> DependentOn
        {
            get { return _dependentOn ?? (_dependentOn = new List<Dependency>()); }
            set { this._dependentOn = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual IList<string> Tags
        {
            get { return _tags ?? (_tags = new List<string>()); }
            set { _tags = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public PluginDescription()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="assembly"></param>
        /// <param name="type"></param>
        public PluginDescription(FileInfo file, Assembly assembly, Type type)
            : this()
        {
            this.PluginAssembly = assembly;
            this.PluginType = type;
            this.PluginFile = file;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public PluginDescription(Type type)
            : this()
        {
            this.PluginType = type;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Instance<T>() where T : class, IPlugin
        {
            object obj = null;
            try
            {
                obj = EngineContext.Current.ContainerManager.Resolve<T>(this.PluginId);
            }
            catch
            {
                EngineContext.Current.ContainerManager.RegisterComponent(typeof(T), this.PluginType, this.PluginId, ComponentLifeStyle.LifetimeScope);
                obj = EngineContext.Current.ContainerManager.Resolve<T>(this.PluginId);
            }
            T t = obj as T;
            if (t != null)
            {
                t.Description = this;
            }
            return t;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IPlugin Instance()
        {
            return this.Instance<IPlugin>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(PluginDescription other)
        {
            if (this.Index != other.Index)
            {
                return this.Index.CompareTo(other.Index);
            }
            return this.PluginName.CompareTo(other.PluginName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            PluginDescription pluginDescription = obj as PluginDescription;
            return pluginDescription != null && this.PluginId != null && this.PluginId.Equals(pluginDescription.PluginId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.PluginName.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.PluginId.GetHashCode();
        }
    }
}