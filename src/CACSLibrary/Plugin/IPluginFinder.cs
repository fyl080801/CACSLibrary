using System;
using System.Collections.Generic;

namespace CACSLibrary.Plugin
{
    /// <summary>
    /// ���Ҳ��
    /// </summary>
    public interface IPluginFinder
    {
        /// <summary>
        /// �������
        /// </summary>
        IPluginManager PluginManager { get; }

        /// <summary>
        /// ��ȡ���в��
        /// </summary>
        /// <typeparam name="T">�������</typeparam>
        /// <param name="installedOnly">ֻ�����Ѱ�װ��</param>
        /// <returns>����б�</returns>
        IEnumerable<T> GetPlugins<T>(bool installedOnly = false) where T : class, IPlugin;

        /// <summary>
        /// ��ȡ���в�������ļ�
        /// </summary>
        /// <param name="installedOnly">ֻ�����Ѱ�װ��</param>
        /// <returns>��������б�</returns>
        IEnumerable<PluginDescription> GetPluginDescriptors(bool installedOnly = false);

        /// <summary>
        /// ��ð���������ϵ�����Ĳ�������ļ�
        /// </summary>
        /// <returns>��������б�</returns>
        IEnumerable<PluginDescription> GetOrderedPlugins();

        /// <summary>
        /// ��ȡָ�����͵Ĳ���������ļ�
        /// </summary>
        /// <typeparam name="T">�������</typeparam>
        /// <param name="installedOnly">ֻ�����Ѱ�װ��</param>
        /// <returns>��������б�</returns>
        IEnumerable<PluginDescription> GetPluginDescriptors<T>(bool installedOnly = false) where T : class, IPlugin;

        /// <summary>
        /// ͨ��Id��ȡ��������ļ�
        /// </summary>
        /// <param name="pluginId">���Id</param>
        /// <param name="installedOnly">ֻ�����Ѱ�װ��</param>
        /// <returns>��������ļ�</returns>
        PluginDescription GetPluginDescriptorById(string pluginId, bool installedOnly = false);

        /// <summary>
        /// ͨ��Id��ȡָ�����͵Ĳ�������ļ�
        /// </summary>
        /// <typeparam name="T">�������</typeparam>
        /// <param name="pluginId">���Id</param>
        /// <param name="installedOnly">ֻ�����Ѱ�װ��</param>
        /// <returns>��������ļ�</returns>
        PluginDescription GetPluginDescriptorById<T>(string pluginId, bool installedOnly = false) where T : class, IPlugin;

        /// <summary>
        /// ������ȥ���
        /// </summary>
        void ReloadPlugins();
    }
}
