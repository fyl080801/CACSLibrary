using System;

namespace CACSLibrary.Profile
{
    /// <summary>
    /// 配置文件对象基类
    /// </summary>
    /// <remarks>
    /// 配置文件下的所有属性都作为配置文件的配置项
    /// <note type="caution">
    /// 使用 <see cref="BinaryProfileProvider"/> 作为配置文件处理类的时必须在属性上加入 Serializable 特性
    /// </note>
    /// </remarks>
    /// <example>
    /// 如何创建配置文件
    /// <code language="cs">
    /// 
    /// //实现一个配置文件基类
    /// public class BaseProfile : ProfileObject
    /// {
    ///     //使用 XML 配置文件处理类将存储路径设在网站的 Profiles 目录下
    ///     public BaseProfile() : base(new XmlProfileProvider(HostingEnvironment.MapPath("~/Profiles")))
    ///     {
    ///     }
    ///     
    ///     public override ProfileObject GetDefault()
    ///     {
    ///         return this;
    ///     }
    /// }
    /// 
    /// </code>
    /// </example>
    [Serializable]
    public abstract class ProfileObject
    {
        [NonSerialized]
        private IProfileProvider _Provider;

        internal IProfileProvider Provider
        {
            get
            {
                if (this._Provider == null)
                {
                    this._Provider = new XmlProfileProvider();
                }
                return this._Provider;
            }
            private set
            {
                this._Provider = value;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ProfileObject()
        {
        }

        /// <summary>
        /// 初始化一个配置文件对象，使用 IProfileProvider
        /// </summary>
        /// <param name="provider">配置文件处理函数</param>
        public ProfileObject(IProfileProvider provider)
        {
            this._Provider = provider;
        }

        internal void Save()
        {
            this.Provider.Save(this);
        }

        internal void Clear()
        {
            this.Provider.Clear(this);
        }

        internal ProfileObject Load()
        {
            ProfileObject profileObject = this.Provider.Load(base.GetType()) as ProfileObject;
            if (profileObject == null)
            {
                profileObject = this.GetDefault();
            }
            profileObject.Provider = this._Provider;
            return profileObject;
        }

        /// <summary>
        /// 获取配置文件的默认值
        /// </summary>
        /// <returns>配置文件对象</returns>
        public abstract ProfileObject GetDefault();
    }
}
