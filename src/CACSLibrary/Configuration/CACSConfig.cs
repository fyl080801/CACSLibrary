using System;
using System.Configuration;

namespace CACSLibrary.Configuration
{
    /// <summary>
    /// �������
    /// </summary>
    /// <remarks>
    /// ����⺯��ʹ�õĻ����������
    /// </remarks>
    public class CACSConfig : ConfigurationSection
    {
        /// <summary>
        /// ��������
        /// </summary>
        [ConfigurationProperty("engineType")]
        public string EngineType
        {
            get { return base["engineType"] as string; }
        }

        /// <summary>
        /// �����ļ�����
        /// </summary>
        [ConfigurationProperty("profileType")]
        public string ProfileType
        {
            get { return base["profileType"] as string; }
        }

        /// <summary>
        /// ��������
        /// </summary>
        [ConfigurationProperty("cacheType")]
        public string CacheType
        {
            get { return base["cacheType"] as string; }
        }
    }
}
