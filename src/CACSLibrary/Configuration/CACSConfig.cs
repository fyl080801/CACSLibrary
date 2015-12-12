using System;
using System.Configuration;

namespace CACSLibrary.Configuration
{
    /// <summary>
    /// 组件配置
    /// </summary>
    /// <remarks>
    /// 定义库函数使用的基础组件类型
    /// </remarks>
    public class CACSConfig : ConfigurationSection
    {
        /// <summary>
        /// 引擎类型
        /// </summary>
        [ConfigurationProperty("engineType")]
        public string EngineType
        {
            get { return base["engineType"] as string; }
        }

        /// <summary>
        /// 配置文件类型
        /// </summary>
        [ConfigurationProperty("profileType")]
        public string ProfileType
        {
            get { return base["profileType"] as string; }
        }

        /// <summary>
        /// 缓存类型
        /// </summary>
        [ConfigurationProperty("cacheType")]
        public string CacheType
        {
            get { return base["cacheType"] as string; }
        }
    }
}
