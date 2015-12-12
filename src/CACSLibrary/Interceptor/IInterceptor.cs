using System;

namespace CACSLibrary.Interceptor
{
    /// <summary>
    /// 拦截器接口
    /// </summary>
    public interface IInterceptor
    {
        /// <summary>
        /// 执行顺序
        /// </summary>
        /// <remarks>
        /// <para>
        ///     描述了当拦截操作执行时处理程序执行顺序
        /// </para>
        /// <para>
        ///     顺序越小在方法执行前越早执行，顺序越大执行越晚
        /// </para>
        /// </remarks>
        int Index
        {
            get;
            set;
        }

        /// <summary>
        /// 创建调用处理接口
        /// </summary>
        /// <returns>调用处理接口</returns>
        ICallHandler BuildCallHandler();
    }
}
