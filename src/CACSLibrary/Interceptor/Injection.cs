using System;
using System.Collections.Generic;

namespace CACSLibrary.Interceptor
{
    /// <summary>
    /// 拦截器封装
    /// </summary>
    /// <remarks>
    /// <note type="caution">
    /// 此类型只能封装 <see cref="System.MarshalByRefObject"/> 对象，WCF服务发布的对象除外。
    /// </note>
    /// </remarks>
    public class Injection
    {
        /// <summary>
        /// 封装一个泛型类型使用拦截器处理标记过的方法
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="instance">实例</param>
        /// <param name="interceptors">附加拦截器</param>
        /// <returns>类型实例</returns>
        public static T Wrap<T>(object instance, params IInterceptor[] interceptors)
        {
            return (T)((object)Injection.Wrap(typeof(T), instance, interceptors));
        }

        /// <summary>
        /// 封装一个类型实例使用拦截器处理标记过的方法
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="instance">实例</param>
        /// <param name="interceptors">附加拦截器</param>
        /// <returns>类型实例</returns>
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
