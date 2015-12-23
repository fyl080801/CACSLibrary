using CACSLibrary.Plugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace CACSLibrary.Web.Plugin
{
    public class PluginFileLoader
    {
        private static readonly string DescriptionRootPath = "description";
        private static readonly string InstalledPluginsRootPath = "InstalledPlugins";

        private static XmlDocument LoadInstalledPluginsFile(string p, string root)
        {
            XmlDocument document = new XmlDocument();
            if (!File.Exists(p))
            {
                XmlDeclaration newChild = document.CreateXmlDeclaration("1.0", "GB2312", null);
                document.AppendChild(newChild);
                XmlElement element = document.CreateElement(InstalledPluginsRootPath);
                document.AppendChild(element);
                document.Save(p);
            }
            document.Load(p);
            return document;
        }

        public static IList<string> ParseInstalledPluginsFile(string p)
        {
            List<string> list = new List<string>();
            XmlNodeList childNodes = LoadInstalledPluginsFile(p, InstalledPluginsRootPath).SelectSingleNode(InstalledPluginsRootPath).ChildNodes;
            foreach (XmlNode node in childNodes)
            {
                if (node.Name == "Plugin")
                {
                    list.Add(node.LastChild.Value);
                }
            }
            return list;
        }

        public static PluginDescription ParsePluginDescriptionFile(string p)
        {
            PluginDescription description = new PluginDescription();
            try
            {
                XmlNodeList childNodes = LoadInstalledPluginsFile(p, DescriptionRootPath).SelectSingleNode(DescriptionRootPath).ChildNodes;
                foreach (XmlNode node in childNodes)
                {
                    if (node.Name == "index")
                    {
                        description.Index = Convert.ToInt32(node.LastChild.Value);
                    }
                    if (node.Name == "pluginFileName")
                    {
                        description.PluginFileName = node.LastChild.Value;
                    }
                    if (node.Name == "pluginId")
                    {
                        description.PluginId = node.LastChild.Value;
                    }
                    if (node.Name == "pluginName")
                    {
                        description.PluginName = node.LastChild == null ? "" : node.LastChild.Value;
                    }
                    if (node.Name == "supportedVersion")
                    {
                        description.SupportedVersion = Version.Parse(node.LastChild.Value);
                    }
                    if (node.Name == "version")
                    {
                        description.Version = Version.Parse(node.LastChild.Value);
                    }
                    if (node.Name == "remark")
                    {
                        description.Remark = node.LastChild != null ? node.LastChild.Value : "";
                    }
                    if (node.Name == "dependentOn")
                    {
                        foreach (XmlNode childNode in node.ChildNodes)
                        {
                            Dependency item = new Dependency
                            {
                                PluginId = childNode.Attributes["pluginId"].Value,
                                Version = Version.Parse(childNode.Attributes["version"].Value)
                            };
                            description.DependentOn.Add(item);
                        }
                    }
                    if (node.Name == "tags")
                    {
                        string[] tags = node.LastChild.Value.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string tag in tags)
                        {
                            description.Tags.Add(tag);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                throw new PluginException(description.PluginId, (int)PluginErrors.Description, "插件描述文件格式不正确", exception);
            }
            return description;
        }

        public static void SaveInstalledPluginsFile(IList<string> installedPluginId, string filePath)
        {
            IList<string> list = ParseInstalledPluginsFile(filePath);
            XmlDocument document = LoadInstalledPluginsFile(filePath, InstalledPluginsRootPath);
            XmlNode node = document.SelectSingleNode(InstalledPluginsRootPath);
            node.RemoveAll();
            foreach (string str in installedPluginId)
            {
                XmlElement newChild = document.CreateElement("Plugin");
                newChild.InnerText = str;
                node.AppendChild(newChild);
            }
            document.Save(filePath);
        }
    }
}

