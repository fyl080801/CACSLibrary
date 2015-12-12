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
    /// XML �����ļ�����
    /// </summary>
    /// <remarks>
    /// �������ļ��� XML �ļ���ʽ�洢����չ��Ϊ config
    /// <note type="tip">
    /// �����ļ���ʹ�û�����ƣ����ļ��������仯ʱ����ֱ�Ӵ��ڴ���ȡ�����ݣ�Ҳ������ʵ������ʱָ��<see cref="CACSLibrary.Caching.ICacheManager"/>�������
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
        /// ��ʼ����
        /// </summary>
        /// <param name="path">·��</param>
        public XmlProfileProvider(string path)
            : this()
        {
            this._path = path;
        }

        /// <summary>
        /// ��ʼ����
        /// </summary>
        /// <param name="path">·��</param>
        /// <param name="isAbsolute">�Ƿ��Ǿ���·��</param>
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
        /// ��ʼ���࣬����������������ʽָ���洢·��
        /// </summary>
        /// <param name="func">����·���ĺ���</param>
        public XmlProfileProvider(Func<string> func)
            : this()
        {
            this._path = func.Invoke();
        }

        /// <summary>
        /// ��ʼ���࣬��ָ������
        /// </summary>
        /// <param name="cacheManager">�������</param>
        public XmlProfileProvider(ICacheManager cacheManager)
        {
            this._cache = cacheManager;
        }

        /// <summary>
        /// ��ʼ���࣬��ָ������
        /// </summary>
        /// <param name="cacheManager">���洦��</param>
        /// <param name="path">·��</param>
        public XmlProfileProvider(ICacheManager cacheManager, string path)
            : this(cacheManager)
        {
            this._path = path;
        }

        /// <summary>
        /// ��ʼ���࣬��ָ������
        /// </summary>
        /// <param name="cacheManager">���洦��</param>
        /// <param name="path">·��</param>
        /// <param name="isAbsolute">�Ƿ��Ǿ���·��</param>
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
        /// ��ʼ���࣬��ָ������
        /// </summary>
        /// <param name="cacheManager">���洦��</param>
        /// <param name="func">����·���ĺ���</param>
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
