using System;

namespace CACSLibrary.Profile
{
    /// <summary>
    /// �����ļ�����
    /// </summary>
    /// <remarks>
    /// �˽ӿ��ṩ�������ļ�����Ļ�������
    /// <note type="caution">
    /// <para>ֻ��<see cref="CACSLibrary.Profile.ProfileObject"/>���͵�������ſ���ʹ�������ļ�����</para>
    /// </note>
    /// </remarks>
    public interface IProfileManager
    {
        /// <summary>
        /// ���һ�����������ļ���
        /// </summary>
        /// <typeparam name="T"><see cref="CACSLibrary.Profile.ProfileObject"/>����</typeparam>
        void Add<T>() where T : ProfileObject;

        /// <summary>
        /// ���һ�������ļ���
        /// </summary>
        /// <param name="configType">��������</param>
        void Add(Type[] configType);

        /// <summary>
        /// ���һ�������ļ�
        /// </summary>
        /// <param name="configs">������������</param>
        void Add(ProfileObject[] configs);

        /// <summary>
        /// ��ȡ���������ļ�
        /// </summary>
        /// <typeparam name="T">��������</typeparam>
        /// <returns>��������</returns>
        T Get<T>() where T : ProfileObject;

        /// <summary>
        /// ��ȡ���������ļ�</summary>
        /// <typeparam name="T">��������</typeparam>
        /// <param name="profile">�����ļ�</param>
        /// <returns>�Ƿ�ɹ���ȡ</returns>
        /// <remarks>
        /// ��������ļ���ȡ�ɹ����� true�����򷵻�false</remarks>
        bool TryGet<T>(out T profile) where T : ProfileObject;

        /// <summary>
        /// ��ȡ�����ļ�
        /// </summary>
        /// <param name="configType">����</param>
        /// <returns>�����ļ�</returns>
        ProfileObject Get(Type configType);

        /// <summary>
        /// ��ȡ�����ļ�</summary>
        /// <param name="configType">����</param>
        /// <param name="profile">�����ļ�</param>
        /// <returns>�Ƿ�ɹ���ȡ</returns>
        /// <remarks>
        /// ��������ļ���ȡ�ɹ����� true�����򷵻�false</remarks>
        bool TryGet(Type configType, out ProfileObject profile);

        /// <summary>
        /// ��ȡ�����ļ�Ĭ��ֵ
        /// </summary>
        /// <typeparam name="T">��������</typeparam>
        /// <returns>�����ļ�</returns>
        T GetDefault<T>() where T : ProfileObject;

        /// <summary>
        /// ��ȡ�����ļ�Ĭ��ֵ
        /// </summary>
        /// <param name="configType">��������</param>
        /// <returns>�����ļ�</returns>
        ProfileObject GetDefault(Type configType);

        /// <summary>
        /// �������ü��ļ�
        /// </summary>
        /// <param name="config">�����ļ�</param>
        void Save(ProfileObject config);

        /// <summary>
        /// ��ԭ�����ļ�ΪĬ��ֵ
        /// </summary>
        /// <param name="config">�����ļ�</param>
        void Clear(ProfileObject config);

        /// <summary>
        /// ����һ�������ļ�
        /// </summary>
        /// <param name="configs">�����ļ�����</param>
        void SaveAll(ProfileObject[] configs);
    }
}
