using CACSLibrary.Caching;
using CACSLibrary.Caching.Expirations;
using CACSLibrary.Infrastructure;
using System;
using System.IO;
using System.Security;
using System.Xml.Serialization;

namespace CACSLibrary.Profile
{
    /// <summary>
    /// XML 配置文件处理
    /// </summary>
    /// <remarks>
    /// 将配置文件以 XML 文件形式存储，扩展名为 config
    /// <note type="tip">
    /// 配置文件会使用缓存机制，在文件不发生变化时可以直接从内存中取出数据，也可以在实例化类时指定<see cref="CACSLibrary.Caching.ICacheManager"/>缓存管理
    /// </note>
    /// </remarks>
    public class XmlProfileProvider : IProfileProvider
    {
        private ICacheManager _cache;
        private string _path;

        /// <summary>
        /// 
        /// </summary>
        public string Path
        {
            get { return this._path; }
        }

        /// <summary>
        /// 
        /// </summary>
        public XmlProfileProvider()
        {
            this._cache = EngineContext.Current.Resolve<ICacheManager>();
        }

        /// <summary>
        /// 初始化类
        /// </summary>
        /// <param name="path">路径</param>
        public XmlProfileProvider(string path)
            : this()
        {
            this._path = path;
        }

        /// <summary>
        /// 初始化类
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="isAbsolute">是否是绝对路径</param>
        public XmlProfileProvider(string path, bool isAbsolute)
            : this()
        {
            if (!isAbsolute)
            {
                this._path = string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, path ?? "");
                return;
            }
            this._path = path;
        }

        /// <summary>
        /// 初始化类，并以匿名方法方形式指定存储路径
        /// </summary>
        /// <param name="func">返回路径的函数</param>
        public XmlProfileProvider(Func<string> func)
            : this()
        {
            this._path = func.Invoke();
        }

        /// <summary>
        /// 初始化类，并指定缓存
        /// </summary>
        /// <param name="cacheManager">缓存管理</param>
        public XmlProfileProvider(ICacheManager cacheManager)
        {
            this._cache = cacheManager;
        }

        /// <summary>
        /// 初始化类，并指定缓存
        /// </summary>
        /// <param name="cacheManager">缓存处理</param>
        /// <param name="path">路径</param>
        public XmlProfileProvider(ICacheManager cacheManager, string path)
            : this(cacheManager)
        {
            this._path = path;
        }

        /// <summary>
        /// 初始化类，并指定缓存
        /// </summary>
        /// <param name="cacheManager">缓存处理</param>
        /// <param name="path">路径</param>
        /// <param name="isAbsolute">是否是绝对路径</param>
        public XmlProfileProvider(ICacheManager cacheManager, string path, bool isAbsolute)
            : this(cacheManager)
        {
            if (!isAbsolute)
            {
                this._path = string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, path ?? "");
                return;
            }
            this._path = path;
        }

        /// <summary>
        /// 初始化类，并指定缓存
        /// </summary>
        /// <param name="cacheManager">缓存处理</param>
        /// <param name="func">返回路径的函数</param>
        public XmlProfileProvider(ICacheManager cacheManager, Func<string> func)
            : this(cacheManager)
        {
            this._path = func.Invoke();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configName"></param>
        /// <returns></returns>
        protected string GetFullPath(string configName)
        {
            return string.Format("{0}\\{1}.config", this._path, configName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configName"></param>
        /// <returns></returns>
        protected object LoadCache(string configName)
        {
            if (this._cache.Contains(configName))
            {
                return this._cache.GetData(configName);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        protected void SaveCache(object config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }
            string name = config.GetType().Name;
            if (this._cache != null)
            {
                this._cache.Add(name, config, CacheItemPriority.NotRemovable, null, new FileDependency(this.GetFullPath(name)));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configType"></param>
        /// <returns></returns>
        public object Load(Type configType)
        {
            string name = configType.Name;
            object obj = this._cache != null ? this.LoadCache(name) : null;
            if (obj == null)
            {
                string fullPath = this.GetFullPath(name);
                if (File.Exists(fullPath))
                {
                    try
                    {
                        using (Stream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            obj = new XmlSerializer(configType).Deserialize(stream);
                        }
                        this.SaveCache(obj);
                    }
                    catch (InvalidOperationException)
                    {
                        return null;
                    }
                    return obj;
                }
            }
            return obj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public void Save(object config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }
            string fullPath = this.GetFullPath(config.GetType().Name);
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(config.GetType());
                if (!string.IsNullOrEmpty(this._path) && !Directory.Exists(this._path))
                {
                    Directory.CreateDirectory(this._path);
                }
                using (Stream stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
                    xmlSerializerNamespaces.Add("", "");
                    xmlSerializer.Serialize(stream, config, xmlSerializerNamespaces);
                }
            }
            catch (SecurityException ex)
            {
                throw new SecurityException(ex.Message, ex.DenySetInstance, ex.PermitOnlySetInstance, ex.Method, ex.Demanded, ex.FirstPermissionThatFailed);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public void Clear(object config)
        {
            string fullPath = this.GetFullPath(config.GetType().Name);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }
}
