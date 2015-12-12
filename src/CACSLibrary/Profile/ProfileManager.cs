using System;
using System.Collections.Generic;

namespace CACSLibrary.Profile
{
    /// <summary>
    /// 默认配置文件管理
    /// </summary>
    /// <remarks>提供了配置文件管理的基本实现</remarks>
    public class ProfileManager : IProfileManager
    {
        Dictionary<Type, ProfileObject> _context;

        /// <summary>
        /// 
        /// </summary>
        public ProfileManager()
        {
            this.Initialize();
        }

        private void Initialize()
        {
            this._context = new Dictionary<Type, ProfileObject>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Add<T>() where T : ProfileObject
        {
            this.Add(new Type[]
			{
				typeof(T)
			});
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configTypes"></param>
        public void Add(params Type[] configTypes)
        {
            if (configTypes == null)
                return;
            List<ProfileObject> list = new List<ProfileObject>();
            for (int i = 0; i < configTypes.Length; i++)
            {
                Type type = configTypes[i];
                if (!this._context.ContainsKey(type))
                {
                    ProfileObject profileObject = Activator.CreateInstance(type) as ProfileObject;
                    if (profileObject != null)
                    {
                        list.Add(profileObject);
                    }
                }
            }
            this.Add(list.ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configs"></param>
        public void Add(params ProfileObject[] configs)
        {
            if (configs == null)
                return;
            for (int i = 0; i < configs.Length; i++)
            {
                ProfileObject profileObject = configs[i];
                Type type = profileObject.GetType();
                if (!this._context.ContainsKey(type))
                {
                    this._context.Add(type, profileObject);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>() where T : ProfileObject
        {
            return this.Get(typeof(T)) as T;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configType"></param>
        /// <returns></returns>
        public ProfileObject Get(Type configType)
        {
            if (!this._context.ContainsKey(configType))
            {
                this.Add(new Type[]
				{
					configType
				});
            }
            return this._context[configType].Load();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="profile"></param>
        /// <returns></returns>
        public bool TryGet<T>(out T profile) where T : ProfileObject
        {
            profile = default(T);
            ProfileObject profileObject = profile;
            bool result = this.TryGet(typeof(T), out profileObject);
            profile = (profileObject as T);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configType"></param>
        /// <param name="profile"></param>
        /// <returns></returns>
        public bool TryGet(Type configType, out ProfileObject profile)
        {
            if (!this._context.ContainsKey(configType))
            {
                profile = null;
                return false;
            }
            profile = this._context[configType].Load();
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetDefault<T>() where T : ProfileObject
        {
            return this.GetDefault(typeof(T)) as T;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configType"></param>
        /// <returns></returns>
        public ProfileObject GetDefault(Type configType)
        {
            if (!this._context.ContainsKey(configType))
            {
                this.Add(configType);
            }
            return this._context[configType].GetDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public void Save(ProfileObject config)
        {
            if (config == null)
            {
                throw new ArgumentException("配置类型为空");
            }
            if (!this._context.ContainsKey(config.GetType()))
            {
                this.Add(config);
            }
            config.Save();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configs"></param>
        public void SaveAll(ProfileObject[] configs)
        {
            for (int i = 0; i < configs.Length; i++)
            {
                ProfileObject config = configs[i];
                this.Save(config);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public void Clear(ProfileObject config)
        {
            if (config == null)
            {
                throw new ArgumentException("配置类型为空");
            }
            if (this._context.ContainsKey(config.GetType()))
            {
                config = config.GetDefault();
                config.Save();
                return;
            }
            throw new Exception("集合中不包含配置类型");
        }
    }
}
