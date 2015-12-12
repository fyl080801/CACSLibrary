using CACSLibrary.Configuration;
using System;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace CACSLibrary.Infrastructure
{
    /// <summary>
    /// 引擎上下文
    /// </summary>
    /// <remarks>
    /// 直接管理引擎的实例
    /// </remarks>
	public class EngineContext
	{
        /// <summary>
        /// 获取当前程序执行的引擎
        /// </summary>
		public static IEngine Current
		{
			get
			{
				if (Singleton<IEngine>.Instance == null)
				{
					EngineContext.Initialize();
				}
				return Singleton<IEngine>.Instance;
			}
		}

        /// <summary>
        /// 将当前加载的引擎初始化
        /// </summary>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public static void Initialize()
		{
			if (Singleton<IEngine>.IsInstanceNull)
			{
				CACSConfig config = ConfigurationManager.GetSection("cacsConfig") as CACSConfig;
				Singleton<IEngine>.Instance = EngineContext.CreateEngine(config);
				if (Singleton<IEngine>.IsInstanceNull)
				{
					throw new CACSException("无法初始化 IEngine");
				}
			}
		}

        /// <summary>
        /// 创建引擎
        /// </summary>
        /// <param name="config">配置</param>
        /// <returns>引擎</returns>
        /// <remarks>根据传递的配置中描述的引擎创建，默认为<see cref="CACSEngine"/></remarks>
		private static IEngine CreateEngine(CACSConfig config)
		{
			if (config == null || string.IsNullOrEmpty(config.EngineType))
			{
				return new CACSEngine();
			}
			Type type = Type.GetType(config.EngineType);
			if (type == null)
			{
				throw new ConfigurationErrorsException("configType 为空");
			}
			if (!typeof(IEngine).IsAssignableFrom(type))
			{
				throw new ConfigurationErrorsException("configType 不是 IEngine");
			}
			return Activator.CreateInstance(type) as IEngine;
		}

        /// <summary>
        /// 重置引擎
        /// </summary>
        /// <param name="engine">引擎</param>
		public static void Reset(IEngine engine)
		{
			Singleton<IEngine>.Instance = engine;
		}
	}
}
