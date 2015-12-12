using System;
using System.Collections.Generic;

namespace CACSLibrary.Infrastructure
{
    /// <summary>
    /// ��������
    /// </summary>
    /// <remarks>
    /// ����������ִ���д洢ֻ��һ��ʵ���Ķ���
    /// <note type="note">
    /// ֻ��ʵ�������Ҹ��ӵ������еĶ���ſ���
    /// </note>
    /// </remarks>
    /// <example>
    /// ���ʹ�õ�������
    /// <code language="cs">
    /// <![CDATA[
    /// 
    /// //��ʼ�������ӵ���������
    /// Singleton<IEngine>.Instance = EngineContext.CreateEngine(config);
    /// 
    /// ]]>
    /// </code>
    /// </example>
    public class Singleton
    {
        private static readonly IDictionary<Type, object> _singletons;

        /// <summary>
        /// ������������
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
    /// ��������
    /// </summary>
    /// <typeparam name="T">����</typeparam>
    /// <remarks>
    /// ���س������ض���������
    /// </remarks>
    public class Singleton<T> : Singleton
    {
        private static T instance;

        /// <summary>
        /// ��������
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
        /// �����Ƿ��Ѹ���
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
