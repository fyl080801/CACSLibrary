using System;

namespace CACSLibrary.Profile
{
    /// <summary>
    /// �����ļ��������
    /// </summary>
    /// <remarks>
    /// �����ļ��µ��������Զ���Ϊ�����ļ���������
    /// <note type="caution">
    /// ʹ�� <see cref="BinaryProfileProvider"/> ��Ϊ�����ļ��������ʱ�����������ϼ��� Serializable ����
    /// </note>
    /// </remarks>
    /// <example>
    /// ��δ��������ļ�
    /// <code language="cs">
    /// 
    /// //ʵ��һ�������ļ�����
    /// public class BaseProfile : ProfileObject
    /// {
    ///     //ʹ�� XML �����ļ������ཫ�洢·��������վ�� Profiles Ŀ¼��
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
        /// ���캯��
        /// </summary>
        public ProfileObject()
        {
        }

        /// <summary>
        /// ��ʼ��һ�������ļ�����ʹ�� IProfileProvider
        /// </summary>
        /// <param name="provider">�����ļ�������</param>
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
        /// ��ȡ�����ļ���Ĭ��ֵ
        /// </summary>
        /// <returns>�����ļ�����</returns>
        public abstract ProfileObject GetDefault();
    }
}
