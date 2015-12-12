using System;
using System.Collections.Generic;

namespace CACSLibrary.Plugin
{
    /// <summary>
    /// ����ӿ�
    /// </summary>
    /// <remarks>
    /// ���������в���Ļ����ӿڣ����Ҫ�ڳ����ж������������������ӿ�
    /// </remarks>
    public interface IPlugin
    {
        /// <summary>
        /// ���������Ϣ
        /// </summary>
        PluginDescription Description { get; set; }

        /// <summary>
        /// ��װ
        /// </summary>
        void Install();

        /// <summary>
        /// ж��
        /// </summary>
        void Uninstall();
    }
}
