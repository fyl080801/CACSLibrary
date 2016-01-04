using CACSLibrary.Caching;
using CACSLibrary.Caching.BackingStoreImplementations;
using CACSLibrary.Configuration;
using CACSLibrary.Profile;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace CACSLibrary.Infrastructure
{
    /// <summary>
    /// 默认引擎
    /// </summary>
    /// <remarks>针对业务和程序运行的需要再初始化时执行一些特定模块的加载</remarks>
    public class CACSEngine : IEngine
    {
        private IContainerManager _containerManager;

        /// <summary>
        /// 对象容器管理器
        /// </summary>
        public IContainerManager ContainerManager
        {
            get { return this._containerManager; }
        }

        /// <summary>
        /// 引擎初始化
        /// </summary>
        /// <param name="config"></param>
        public virtual void Initialize(CACSConfig config)
        {
            //初始化对象容器
            this._containerManager = this.InitializeContainer();
            //注册类型查找
            this.RegisterTypeFinder();
            //注册设置信息
            this.RegisterConfig(config);
            //注册引擎
            this.RegisterEngine();
            //注册缓存
            this.RegisterCache(config);
            //注册配置文件管理
            this.RegisterProfile(config);
            //注册容器依赖项
            this.RegisterDependency();
            //运行启动任务
            this.RunStartupTasks();
        }

        /// <summary>
        /// 初始化对象容器
        /// </summary>
        /// <returns>对象容器</returns>
        protected virtual IContainerManager InitializeContainer()
        {
            EngineConfig engineConfig = ConfigurationManager.GetSection("engineConfig") as EngineConfig;
            if (engineConfig == null || engineConfig.ContainerType == null || string.IsNullOrEmpty(engineConfig.ContainerType))
            {
                return (IContainerManager)Activator.CreateInstance(Type.GetType("CACSLibrary.Autofac.ContainerManager, CACSLibrary.Autofac"));
            }
            Type containerType = Type.GetType(engineConfig.ContainerType);
            if (containerType == null)
            {
                throw new ConfigurationErrorsException("ContainerManager 为空");
            }
            return (IContainerManager)Activator.CreateInstance(containerType);
        }

        /// <summary>
        /// 返回指定类型的实例
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>类型的实例</returns>
        public T Resolve<T>() where T : class
        {
            return this.ContainerManager.Resolve<T>();
        }

        /// <summary>
        /// 返回指定类型的实例
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>类型的实例</returns>
        public object Resolve(Type type)
        {
            return this.ContainerManager.Resolve(type);
        }

        /// <summary>
        /// 返回指定类型的所有实例
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>实例</returns>
        public T[] ResolveAll<T>()
        {
            return this.ContainerManager.ResolveAll<T>();
        }

        /// <summary>
        /// 注册类型查找
        /// </summary>
        protected virtual void RegisterTypeFinder()
        {
            this.ContainerManager.RegisterComponent<ITypeFinder, AppDomainTypeFinder>(typeof(AppDomainTypeFinder).FullName, ComponentLifeStyle.Singleton);
        }

        /// <summary>
        /// 注册设置信息
        /// </summary>
        /// <param name="config">设置信息</param>
        protected virtual void RegisterConfig(CACSConfig config)
        {
            if (config != null)
            {
                this.ContainerManager.RegisterComponentInstance<CACSConfig>(config, config.GetType().FullName, ComponentLifeStyle.Singleton);
                return;
            }
            this.ContainerManager.RegisterComponent<CACSConfig>(typeof(CACSConfig).FullName, ComponentLifeStyle.Singleton);
        }

        /// <summary>
        /// 注册引擎
        /// </summary>
        protected virtual void RegisterEngine()
        {
            this.ContainerManager.RegisterComponentInstance<IEngine>(this, this.GetType().FullName, ComponentLifeStyle.Singleton);
        }

        /// <summary>
        /// 注册缓存管理
        /// </summary>
        /// <param name="configuration">配置</param>
        protected virtual void RegisterCache(CACSConfig configuration)
        {
            if (configuration == null || string.IsNullOrEmpty(configuration.CacheType))
            {
                CacheConfig cacheConfig = ConfigurationManager.GetSection("cacheConfig") as CacheConfig;
                Cache cache;
                int numberToRemoveWhenScavenging;
                int maximumElementsInCacheBeforeScavenging;
                int expirationPollFrequencyInMilliSeconds;
                if (cacheConfig != null && string.IsNullOrEmpty(cacheConfig.BackingStore))
                {
                    Type type = Type.GetType(cacheConfig.BackingStore);
                    if (type == null)
                    {
                        throw new ConfigurationErrorsException("IBackingStore 配置名称不能为空");
                    }
                    if (!typeof(IBackingStore).IsAssignableFrom(type))
                    {
                        throw new ConfigurationErrorsException(type.FullName + "不是 IBackingStore");
                    }
                    cache = new Cache(Activator.CreateInstance(type) as IBackingStore);
                    numberToRemoveWhenScavenging = cacheConfig.NumberToRemoveWhenScavenging;
                    maximumElementsInCacheBeforeScavenging = cacheConfig.MaximumElementsInCacheBeforeScavenging;
                    expirationPollFrequencyInMilliSeconds = cacheConfig.ExpirationPollTimeout;
                }
                else
                {
                    cache = new Cache(new StaticBackingStore());
                    numberToRemoveWhenScavenging = 8;
                    maximumElementsInCacheBeforeScavenging = 16;
                    expirationPollFrequencyInMilliSeconds = 128;
                }
                this.ContainerManager.RegisterComponentInstance<ICacheManager>(
                    new CacheManager(
                        cache,
                        new BackgroundScheduler(
                            new ExpirationTask(cache),
                            new ScavengerTask(numberToRemoveWhenScavenging, maximumElementsInCacheBeforeScavenging, cache)),
                        new ExpirationPollTimer(expirationPollFrequencyInMilliSeconds)),
                    typeof(CacheManager).FullName,
                    ComponentLifeStyle.Singleton);
                return;
            }
            Type cacheType = Type.GetType(configuration.CacheType);
            if (cacheType == null)
            {
                throw new ConfigurationErrorsException("profileType 为空");
            }
            if (typeof(ICacheManager).IsAssignableFrom(cacheType))
            {
                throw new ConfigurationErrorsException("profileType 不是 IProfileManager");
            }
            this.ContainerManager.RegisterComponent(typeof(ICacheManager), cacheType, cacheType.FullName, ComponentLifeStyle.Singleton);
        }

        /// <summary>
        /// 注册配置文件管理
        /// </summary>
        /// <param name="configuration"></param>
        protected virtual void RegisterProfile(CACSConfig configuration)
        {
            if (configuration == null || string.IsNullOrEmpty(configuration.ProfileType))
            {
                this.ContainerManager.RegisterComponent<IProfileManager, ProfileManager>(typeof(ProfileManager).FullName, ComponentLifeStyle.Singleton);
                return;
            }
            Type profileType = Type.GetType(configuration.ProfileType);
            if (profileType == null)
            {
                throw new ConfigurationErrorsException("profileType 为空");
            }
            if (typeof(IProfileManager).IsAssignableFrom(profileType))
            {
                throw new ConfigurationErrorsException(profileType.FullName + " 不是 IProfileManager");
            }
            this.ContainerManager.RegisterComponent(typeof(IProfileManager), profileType, profileType.FullName, ComponentLifeStyle.Singleton);
        }

        /// <summary>
        /// 注册对象依赖项
        /// </summary>
        protected virtual void RegisterDependency()
        {
            var list = this.FindInstances<IDependencyRegister>().OrderBy(m => m.Level).ToList();
            list.ForEach(delegate(IDependencyRegister m)
            {
                m.Register(this.ContainerManager, this.ContainerManager.Resolve<ITypeFinder>());
            });
        }

        /// <summary>
        /// 运行启动任务
        /// </summary>
        protected virtual void RunStartupTasks()
        {
            var list = this.FindInstances<IStartupTask>().OrderBy(m => m.Level).ToList();
            foreach (IStartupTask task in list)
            {
                task.Execute();
            }
        }

        private IList<T> FindInstances<T>()
        {
            ITypeFinder typeFinder = this._containerManager.Resolve<ITypeFinder>();
            IEnumerable<Type> enumerable = typeFinder.FindClassesOfType<T>(true);
            List<T> list = new List<T>();
            foreach (Type current in enumerable)
            {
                T item = (T)Activator.CreateInstance(current);
                list.Add(item);
            }
            return list;
        }
    }
}
