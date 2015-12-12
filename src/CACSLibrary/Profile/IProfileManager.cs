using System;

namespace CACSLibrary.Profile
{
    /// <summary>
    /// 配置文件管理
    /// </summary>
    /// <remarks>
    /// 此接口提供了配置文件管理的基本方法
    /// <note type="caution">
    /// <para>只有<see cref="CACSLibrary.Profile.ProfileObject"/>类型的派生类才可以使用配置文件管理</para>
    /// </note>
    /// </remarks>
    public interface IProfileManager
    {
        /// <summary>
        /// 添加一个泛型配置文件类
        /// </summary>
        /// <typeparam name="T"><see cref="CACSLibrary.Profile.ProfileObject"/>类型</typeparam>
        void Add<T>() where T : ProfileObject;

        /// <summary>
        /// 添加一个配置文件类
        /// </summary>
        /// <param name="configType">配置类型</param>
        void Add(Type[] configType);

        /// <summary>
        /// 添加一组配置文件
        /// </summary>
        /// <param name="configs">配置类型数组</param>
        void Add(ProfileObject[] configs);

        /// <summary>
        /// 获取泛型配置文件
        /// </summary>
        /// <typeparam name="T">配置类型</typeparam>
        /// <returns>配置类型</returns>
        T Get<T>() where T : ProfileObject;

        /// <summary>
        /// 获取泛型配置文件</summary>
        /// <typeparam name="T">配置类型</typeparam>
        /// <param name="profile">配置文件</param>
        /// <returns>是否成功获取</returns>
        /// <remarks>
        /// 如果配置文件获取成功返回 true，否则返回false</remarks>
        bool TryGet<T>(out T profile) where T : ProfileObject;

        /// <summary>
        /// 获取配置文件
        /// </summary>
        /// <param name="configType">类型</param>
        /// <returns>配置文件</returns>
        ProfileObject Get(Type configType);

        /// <summary>
        /// 获取配置文件</summary>
        /// <param name="configType">类型</param>
        /// <param name="profile">配置文件</param>
        /// <returns>是否成功获取</returns>
        /// <remarks>
        /// 如果配置文件获取成功返回 true，否则返回false</remarks>
        bool TryGet(Type configType, out ProfileObject profile);

        /// <summary>
        /// 获取配置文件默认值
        /// </summary>
        /// <typeparam name="T">配置类型</typeparam>
        /// <returns>配置文件</returns>
        T GetDefault<T>() where T : ProfileObject;

        /// <summary>
        /// 获取配置文件默认值
        /// </summary>
        /// <param name="configType">配置类型</param>
        /// <returns>配置文件</returns>
        ProfileObject GetDefault(Type configType);

        /// <summary>
        /// 保存配置及文件
        /// </summary>
        /// <param name="config">配置文件</param>
        void Save(ProfileObject config);

        /// <summary>
        /// 还原配置文件为默认值
        /// </summary>
        /// <param name="config">配置文件</param>
        void Clear(ProfileObject config);

        /// <summary>
        /// 保存一组配置文件
        /// </summary>
        /// <param name="configs">配置文件数组</param>
        void SaveAll(ProfileObject[] configs);
    }
}
