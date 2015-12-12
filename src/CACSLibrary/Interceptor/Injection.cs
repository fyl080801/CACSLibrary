using System;
using System.Collections.Generic;

namespace CACSLibrary.Interceptor
{
    /// <summary>
    /// ��������װ
    /// </summary>
    /// <remarks>
    /// <note type="caution">
    /// ������ֻ�ܷ�װ <see cref="System.MarshalByRefObject"/> ����WCF���񷢲��Ķ�����⡣
    /// </note>
    /// </remarks>
    public class Injection
    {
        /// <summary>
        /// ��װһ����������ʹ�������������ǹ��ķ���
        /// </summary>
        /// <typeparam name="T">����</typeparam>
        /// <param name="instance">ʵ��</param>
        /// <param name="interceptors">����������</param>
        /// <returns>����ʵ��</returns>
        public static T Wrap<T>(object instance, params IInterceptor[] interceptors)
        {
            return (T)((object)Injection.Wrap(typeof(T), instance, interceptors));
        }

        /// <summary>
        /// ��װһ������ʵ��ʹ�������������ǹ��ķ���
        /// </summary>
        /// <param name="type">����</param>
        /// <param name="instance">ʵ��</param>
        /// <param name="interceptors">����������</param>
        /// <returns>����ʵ��</returns>
        public static object Wrap(Type type, object instance, params IInterceptor[] interceptors)
        {
            List<ICallHandler> list = new List<ICallHandler>();
            if (interceptors != null)
            {
                for (int i = 0; i < interceptors.Length; i++)
                {
                    IInterceptor interceptor = interceptors[i];
                    list.Add(interceptor.BuildCallHandler());
                }
            }
            ICallHandler[] callhandlers = null;
            if (list.Count > 0)
            {
                callhandlers = list.ToArray();
            }
            InterceptorProxy interceptorProxy = new InterceptorProxy(type, instance, callhandlers);
            return interceptorProxy.GetTransparentProxy();
        }
    }
}
