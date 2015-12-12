using System;

namespace CACSLibrary.Profile
{
    /// <summary>
    /// 配置文件处理接口
    /// </summary>
    /// <remarks>
    /// 此接口定义了一个配置文件的基础方法，可通过实现</remarks>
	public interface IProfileProvider
	{
        /// <summary>
        /// 读取配置文件
        /// </summary>
        /// <param name="configType">配置文件类型</param>
        /// <returns>配置文件对象</returns>
		object Load(Type configType);

        /// <summary>
        /// 保存配置文件
        /// </summary>
        /// <param name="config">配置文件对象</param>
		void Save(object config);

        /// <summary>
        /// 清除配置文件
        /// </summary>
        /// <param name="config">配置文件对象</param>
		void Clear(object config);
	}
}
