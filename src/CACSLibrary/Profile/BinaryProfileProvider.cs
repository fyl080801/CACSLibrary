using CACSLibrary.Component;
using System;
using System.IO;
using System.Runtime.Serialization;
namespace CACSLibrary.Profile
{
    /// <summary>
    /// 序列化存储配置文件
    /// </summary>
    /// <remarks>
    /// 所有以序列化数据存储配置文件处理的基类
    /// </remarks>
    public abstract class BinaryProfileProvider : IProfileProvider
    {
        private IProfileEncryptionProvider _Encryption;

        /// <summary>
        /// 
        /// </summary>
        public IProfileEncryptionProvider Encryption
        {
            get { return this._Encryption; }
        }

        /// <summary>
        /// 
        /// </summary>
        public BinaryProfileProvider()
        {
            this._Encryption = new NullProfileEncryption();
        }

        /// <summary>
        /// 初始化类型
        /// </summary>
        /// <param name="encryption">加密处理</param>
        public BinaryProfileProvider(IProfileEncryptionProvider encryption)
        {
            this._Encryption = encryption;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configType"></param>
        /// <returns></returns>
        public object Load(Type configType)
        {
            byte[] array = this.LoadConfig(configType.Name);
            if (array != null && array.Length > 0)
            {
                try
                {
                    array = this._Encryption.Dencrypt(array);
                    return SerializationHelper.ToObject(array);
                }
                catch (SerializationException ex)
                {
                    throw ex;
                }
            }
            ProfileObject profileObject = Activator.CreateInstance(configType) as ProfileObject;
            if (profileObject == null)
            {
                throw new ArgumentException("无法加载配置");
            }
            return profileObject.GetDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public void Save(object config)
        {
            using (new MemoryStream())
            {
                byte[] array = SerializationHelper.ToBytes(config);
                array = this._Encryption.Encrypt(array);
                this.SaveConfig(array, config.GetType().Name);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public void Clear(object config)
        {
            this.ClearConfig(config.GetType().Name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="configName"></param>
        protected abstract void SaveConfig(byte[] config, string configName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configName"></param>
        protected abstract void ClearConfig(string configName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configName"></param>
        /// <returns></returns>
        protected abstract byte[] LoadConfig(string configName);
    }
}
