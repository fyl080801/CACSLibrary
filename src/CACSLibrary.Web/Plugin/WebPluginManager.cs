using CACSLibrary.Plugin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using System.Web.Hosting;

namespace CACSLibrary.Web.Plugin
{
    public class WebPluginManager : BasePluginManager
    {
        private bool _clearShadowDirectoryOnStartup;
        private readonly string _installedPluginsFilePath = "~/App_Data/InstalledPlugins.xml";
        private readonly string _pluginsPath = "~/Plugins";
        private DirectoryInfo _shadowCopyFolder;
        private readonly string _shadowCopyPath = "~/Plugins/bin";

        private IEnumerable<KeyValuePair<FileInfo, PluginDescription>> GetDescriptionFilesAndDescriptors(DirectoryInfo pluginFolder)
        {
            if (pluginFolder == null)
            {
                throw new ArgumentNullException("pluginFolder");
            }
            List<KeyValuePair<FileInfo, PluginDescription>> list = new List<KeyValuePair<FileInfo, PluginDescription>>();
            foreach (FileInfo info in pluginFolder.GetFiles("Description.xml", SearchOption.AllDirectories))
            {
                PluginDescription description = PluginFileLoader.ParsePluginDescriptionFile(info.FullName);
                if (description != null)
                {
                    list.Add(new KeyValuePair<FileInfo, PluginDescription>(info, description));
                }
            }
            list.Sort((Comparison<KeyValuePair<FileInfo, PluginDescription>>)((firstPair, nextPair) => firstPair.Value.Index.CompareTo(nextPair.Value.Index)));
            return list;
        }

        private string GetInstalledPluginsFilePath()
        {
            return HostingEnvironment.MapPath(this._installedPluginsFilePath);
        }

        public void Initialize()
        {
            DirectoryInfo pluginFolder = new DirectoryInfo(HostingEnvironment.MapPath(this._pluginsPath));
            this._shadowCopyFolder = new DirectoryInfo(HostingEnvironment.MapPath(this._shadowCopyPath));
            List<PluginDescription> referencedPlugins = new List<PluginDescription>();
            List<string> incompatiblePlugins = new List<string>();
            this._clearShadowDirectoryOnStartup = (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ClearPluginsShadowDirectoryOnStartup"]) && Convert.ToBoolean(ConfigurationManager.AppSettings["ClearPluginsShadowDirectoryOnStartup"]));
            try
            {
                IList<string> installedPluginSystemNames = PluginFileLoader.ParseInstalledPluginsFile(this.GetInstalledPluginsFilePath());
                Directory.CreateDirectory(pluginFolder.FullName);
                Directory.CreateDirectory(this._shadowCopyFolder.FullName);
                FileInfo[] binFiles = this._shadowCopyFolder.GetFiles("*", SearchOption.AllDirectories);
                if (this._clearShadowDirectoryOnStartup)
                {
                    for (int i = 0; i < binFiles.Length; i++)
                    {
                        try
                        {
                            File.Delete(binFiles[i].FullName);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                //获取所有插件文件
                List<DirectoryInfo> pluginLocalFolders = pluginFolder.GetDirectories().ToList<DirectoryInfo>();
                List<FileInfo> pluginFiles = new List<FileInfo>();
                foreach (DirectoryInfo pluginLocalFolder in pluginLocalFolders)
                {
                    List<FileInfo> pluginFile = (
                        from x in pluginLocalFolder.GetFiles("*.dll", SearchOption.AllDirectories)
                        where !(
                            from q in binFiles
                            select q.FullName).Contains(x.FullName)
                        where this.IsPackagePluginFolder(x.Directory)
                        select x).ToList<FileInfo>();
                    pluginFiles.AddRange(pluginFile);
                }
                //找到所有插件的头文件
                IEnumerable<KeyValuePair<FileInfo, PluginDescription>> descriptorCollection = this.GetDescriptionFilesAndDescriptors(pluginFolder);
                Dictionary<string, Assembly> pluginFileCollection = new Dictionary<string, Assembly>();
                using (IEnumerator<FileInfo> fileEnumerator = (
                    from x in pluginFiles
                    where !this.IsAlreadyLoaded(x)
                    select x).GetEnumerator())
                {
                    while (fileEnumerator.MoveNext())
                    {
                        FileInfo plugin = fileEnumerator.Current;
                        Assembly pluginAssembly = this.PerformFileDeploy(plugin);
                        if (descriptorCollection.Any((KeyValuePair<FileInfo, PluginDescription> m) => m.Value.PluginFileName == plugin.Name))
                        {
                            pluginFileCollection.Add(plugin.Name, pluginAssembly);
                        }
                    }
                }
                //组合插件描述文件
                foreach (KeyValuePair<FileInfo, PluginDescription> dfd in descriptorCollection)
                {
                    PluginDescription pluginDescriptor = dfd.Value;
#if NET4_5_1
                    if (string.IsNullOrWhiteSpace(pluginDescriptor.PluginId))
                        throw new CACSException("插件 Id 为空");
#else
                    if (string.IsNullOrEmpty(pluginDescriptor.PluginId))
                        throw new CACSException("插件 Id 为空");
#endif
                    if (referencedPlugins.Contains(pluginDescriptor))
                    {
                        throw new CACSException("插件重复，Id：" + pluginDescriptor.PluginId);
                    }
                    pluginDescriptor.Installed = ((
                        from x in installedPluginSystemNames.ToList<string>()
                        where x.Equals(pluginDescriptor.PluginId, StringComparison.InvariantCultureIgnoreCase)
                        select x)
                        .FirstOrDefault<string>() != null);
                    try
                    {
                        FileInfo mainPluginFile = (
                            from x in pluginFiles
                            where x.Name.Equals(pluginDescriptor.PluginFileName, StringComparison.InvariantCultureIgnoreCase)
                            select x).FirstOrDefault<FileInfo>();
                        pluginDescriptor.PluginFile = mainPluginFile;
                        pluginDescriptor.PluginAssembly = pluginFileCollection[pluginDescriptor.PluginFileName];
                        Type[] types = pluginDescriptor.PluginAssembly.GetTypes();
                        for (int i = 0; i < types.Length; i++)
                        {
                            Type t = types[i];
                            if (typeof(IPlugin).IsAssignableFrom(t) && !t.IsInterface && t.IsClass && !t.IsAbstract)
                            {
                                pluginDescriptor.PluginType = t;
                                break;
                            }
                        }
                        referencedPlugins.Add(pluginDescriptor);
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        string msg = string.Empty;
                        Exception[] loaderExceptions = ex.LoaderExceptions;
                        for (int i = 0; i < loaderExceptions.Length; i++)
                        {
                            Exception e = loaderExceptions[i];
                            msg = msg + e.Message + Environment.NewLine;
                        }
                        Exception fail = new Exception(msg, ex);
                        throw fail;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Empty;
                for (Exception e = ex; e != null; e = e.InnerException)
                {
                    msg = msg + e.Message + Environment.NewLine;
                }
                Exception fail = new Exception(msg, ex);
                throw fail;
            }
            //referencedPlugins.ForEach(referencedPlugin =>
            //{
            //    this.ReferencedPlugins.Add(referencedPlugin.PluginId, referencedPlugin);
            //});
            this.ReferencedPlugins = referencedPlugins;
        }

        private FileInfo InitializeFullTrust(FileInfo plug, DirectoryInfo shadowCopyPlugFolder)
        {
            FileInfo info = new FileInfo(Path.Combine(shadowCopyPlugFolder.FullName, plug.Name));
            try
            {
                File.Copy(plug.FullName, info.FullName, true);
            }
            catch (IOException)
            {
                try
                {
                    string destFileName = info.FullName + Guid.NewGuid().ToString("N") + ".old";
                    File.Move(info.FullName, destFileName);
                }
                catch (IOException)
                {
                }
                File.Copy(plug.FullName, info.FullName, true);
            }
            return info;
        }

        private FileInfo InitializeMediumTrust(FileInfo plug, DirectoryInfo shadowCopyPlugFolder)
        {
            bool flag = true;
            FileInfo info = new FileInfo(Path.Combine(shadowCopyPlugFolder.FullName, plug.Name));
            if (info.Exists)
            {
                if (info.CreationTimeUtc.Ticks >= plug.CreationTimeUtc.Ticks)
                {
                    flag = false;
                }
                else
                {
                    File.Delete(info.FullName);
                }
            }
            if (flag)
            {
                try
                {
                    File.Copy(plug.FullName, info.FullName, true);
                }
                catch (IOException)
                {
                    try
                    {
                        string destFileName = info.FullName + Guid.NewGuid().ToString("N") + ".old";
                        File.Move(info.FullName, destFileName);
                    }
                    catch (IOException)
                    {
                    }
                    File.Copy(plug.FullName, info.FullName, true);
                }
            }
            return info;
        }

        private bool IsAlreadyLoaded(FileInfo fileInfo)
        {
            try
            {
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.FullName);
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    string str = assembly.FullName.Split(new char[] { ',' }).FirstOrDefault<string>();
                    if (fileNameWithoutExtension.Equals(str, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            catch (Exception)
            {
            }
            return false;
        }

        private bool IsPackagePluginFolder(DirectoryInfo folder)
        {
            if (folder == null)
            {
                return false;
            }
            if (folder.Parent == null)
            {
                return false;
            }
            if (folder.Name.Equals("bin", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }
            if (!folder.Parent.Name.Equals("Plugins", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }
            return true;
        }

        private Assembly PerformFileDeploy(FileInfo plug)
        {
            FileInfo info;
            if (plug.Directory.Parent == null)
            {
                throw new InvalidOperationException();
            }
            if (WebCommonHelper.GetTrustLevel() != AspNetHostingPermissionLevel.Unrestricted)
            {
                DirectoryInfo shadowCopyPlugFolder = Directory.CreateDirectory(this._shadowCopyFolder.FullName);
                info = this.InitializeMediumTrust(plug, shadowCopyPlugFolder);
            }
            else
            {
                string dynamicDirectory = AppDomain.CurrentDomain.DynamicDirectory;
                info = this.InitializeFullTrust(plug, new DirectoryInfo(dynamicDirectory));
            }
            Assembly assembly = Assembly.Load(AssemblyName.GetAssemblyName(info.FullName));
            BuildManager.AddReferencedAssembly(assembly);
            return assembly;
        }

        //public override string GetPluginPath(string id)
        //{
        //    if (!this.ReferencedPlugins.ContainsKey(id))
        //        throw new CACSException("不包含模块：" + id);
        //    var plugin = this.ReferencedPlugins[id];
        //    if (plugin == null)
        //        throw new CACSException("插件不存才：" + id);
        //    return plugin.PluginFile.DirectoryName.Substring(plugin.PluginFile.DirectoryName.LastIndexOf("\\"));
        //}

        protected override void PluginInstall(string pluginId)
        {
            if (string.IsNullOrEmpty(pluginId))
            {
                throw new ArgumentNullException("pluginId");
            }
            string path = HostingEnvironment.MapPath(this._installedPluginsFilePath);
            IList<string> source = PluginFileLoader.ParseInstalledPluginsFile(this.GetInstalledPluginsFilePath());
            if ((from x in source.ToList<string>()
                 where x.Equals(pluginId, StringComparison.InvariantCultureIgnoreCase)
                 select x).FirstOrDefault<string>() == null)
            {
                source.Add(pluginId);
            }
            PluginFileLoader.SaveInstalledPluginsFile(source, path);
        }

        protected override void PluginUninstall(string pluginId)
        {
            if (string.IsNullOrEmpty(pluginId))
            {
                throw new ArgumentNullException("pluginId");
            }
            string path = HostingEnvironment.MapPath(this._installedPluginsFilePath);
            if (!File.Exists(path))
            {
                using (File.Create(path))
                {
                }
            }
            IList<string> source = PluginFileLoader.ParseInstalledPluginsFile(this.GetInstalledPluginsFilePath());
            if ((from x in source.ToList<string>()
                 where x.Equals(pluginId, StringComparison.InvariantCultureIgnoreCase)
                 select x).FirstOrDefault<string>() != null)
            {
                source.Remove(pluginId);
            }
            PluginFileLoader.SaveInstalledPluginsFile(source, path);
        }

        protected override void AllPluginUninstall()
        {
            string path = HostingEnvironment.MapPath(this._installedPluginsFilePath);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
