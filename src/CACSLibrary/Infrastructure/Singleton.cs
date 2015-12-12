using System;
using System.Collections.Generic;

namespace CACSLibrary.Infrastructure
{
    /// <summary>
    /// 单例容器
    /// </summary>
    /// <remarks>
    /// 在整个程序执行中存储只有一个实例的对象
    /// <note type="note">
    /// 只有实例化并且附加到集合中的对象才可用
    /// </note>
    /// </remarks>
    /// <example>
    /// 如何使用单例容器
    /// <code language="cs">
    /// <![CDATA[
    /// 
    /// //初始化并附加到单例集合
    /// Singleton<IEngine>.Instance = EngineContext.CreateEngine(config);
    /// 
    /// ]]>
    /// </code>
    /// </example>
    public class Singleton
    {
        private static readonly IDictionary<Type, object> _singletons;

        /// <summary>
        /// 单例容器集合
        /// </summary>
        public static IDictionary<Type, object> Singletons
        {
            get
            {
                return Singleton._singletons;
            }
        }

        static Singleton()
        {
            Singleton._singletons = new Dictionary<Type, object>();
        }
    }

    /// <summary>
    /// 单例容器
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <remarks>
    /// 返回程序中特定单例类型
    /// </remarks>
    public class Singleton<T> : Singleton
    {
        private static T instance;

        /// <summary>
        /// 单例对象
        /// </summary>
        public static T Instance
        {
            get
            {
                return Singleton<T>.instance;
            }
            set
            {
                Singleton<T>.instance = value;
                Singleton.Singletons[typeof(T)] = value;
            }
        }

        /// <summary>
        /// 对象是否已附加
        /// </summary>
        public static bool IsInstanceNull
        {
            get
            {
                return Singleton<T>.Instance == null;
            }
        }
    }
}
