using CACSLibrary.Caching;
using CACSLibrary.Caching.Expirations;
using CACSLibrary.Infrastructure;
using System;
using System.IO;
using System.Threading;
namespace CACSLibrary.Profile
{
    /// <summary>
    /// 加密文件配置文件处理
    /// </summary>
    /// <remarks>将配置文件序列化后存储为文件，初始化类时可以指定是否加密处理存储内容</remarks>
    public class BinaryFileProfileProvider : BinaryProfileProvider
    {
        private ICacheManager _cache;
        private string _Path;

        /// <summary>
        /// 
        /// </summary>
        public string Path
        {
            get
            {
                return this._Path;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public BinaryFileProfileProvider()
        {
            this._cache = EngineContext.Current.Resolve<ICacheManager>();
        }

        /// <summary>
        /// 初始化类
        /// </summary>
        /// <param name="path">存储路径</param>
        public BinaryFileProfileProvider(string path)
            : this()
        {
            this.InitConfigPath(path, true);
        }

        /// <summary>
        /// 初始化类
        /// </summary>
        /// <param name="path">存储路径</param>
        /// <param name="isAbsolute">是否是绝对路径</param>
        public BinaryFileProfileProvider(string path, bool isAbsolute)
            : this()
        {
            this.InitConfigPath(path, isAbsolute);
        }

        /// <summary>
        /// 初始化类
        /// </summary>
        /// <param name="path">存储路径</param>
        /// <param name="isAbsolute">是否是绝对路径</param>
        /// <param name="encryption">加密处理</param>
        public BinaryFileProfileProvider(string path, bool isAbsolute, IProfileEncryptionProvider encryption)
            : base(encryption)
        {
            this.InitConfigPath(path, isAbsolute);
            this._cache = EngineContext.Current.Resolve<ICacheManager>();
        }

        private void InitConfigPath(string path, bool isAbsolute)
        {
            if (!isAbsolute)
            {
                this._Path = string.Format("{0}{1}", Thread.GetDomain().BaseDirectory, path ?? "");
                return;
            }
            this._Path = path;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configName"></param>
        /// <returns></returns>
        protected byte[] LoadCache(string configName)
        {
            if (this._cache.Contains(configName))
            {
                return this._cache.GetData(configName) as byte[];
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="configName"></param>
        protected void SaveCache(byte[] config, string configName)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }
            this._cache.Add(configName, config, CacheItemPriority.NotRemovable, null, new ICacheItemExpiration[]
			{
				new FileDependency(this.GetFullPath(configName))
			});
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configName"></param>
        /// <returns></returns>
        protected string GetFullPath(string configName)
        {
            return string.Format("{0}\\{1}.dat", this._Path, configName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="configName"></param>
        protected override void SaveConfig(byte[] config, string configName)
        {
            string fullPath = this.GetFullPath(configName);
            if (!Directory.Exists(this._Path))
            {
                Directory.CreateDirectory(this._Path);
            }
            using (Stream stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                stream.Write(config, 0, config.Length);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configName"></param>
        /// <returns></returns>
        protected override byte[] LoadConfig(string configName)
        {
            byte[] array = this.LoadCache(configName);
            if (array == null)
            {
                string fullPath = this.GetFullPath(configName);
                if (File.Exists(fullPath))
                {
                    using (Stream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                    {
                        array = new byte[stream.Length];
                        stream.Read(array, 0, array.Length);
                    }
                    this.SaveCache(array, configName);
                }
            }
            return array;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configName"></param>
        protected override void ClearConfig(string configName)
        {
            string fullPath = this.GetFullPath(configName);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }
}
