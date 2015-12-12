using System;

namespace CACSLibrary.Interceptor
{
    /// <summary>
    /// 调用处理接口
    /// </summary>
    /// <remarks>
    /// 用于具体实现拦截器操作
    /// </remarks>
    public interface ICallHandler
    {
        /// <summary>
        /// 执行顺序
        /// </summary>
        /// <seealso cref="CACSLibrary.Interceptor.IInterceptor.Index"/>
        int Index
        {
            get;
            set;
        }

        /// <summary>
        /// 忽略处理程序异常
        /// </summary>
        /// <remarks>
        /// 当 PreInvoke 执行时如果出现异常则继续执行方法
        /// </remarks>
        bool IgnoreCallHandlerException
        {
            get;
        }

        /// <summary>
        /// 忽略方法异常
        /// </summary>
        /// <remarks>
        /// 当方法执行时出现异常，无视异常继续执行AftInvoke
        /// </remarks>
        bool IgnoreMethodException
        {
            get;
        }

        /// <summary>
        /// 在方法执行前执行的操作
        /// </summary>
        /// <param name="context">代理上下文</param>
        /// <returns>返回一个供拦截器后续执行时需要共享的对象</returns>
        object PreInvoke(ProxyContext context);

        /// <summary>
        /// 在方法执行后执行的操作
        /// </summary>
        /// <param name="context">代理上下文</param>
        /// <param name="objState">PreInvoke 中共享对象</param>
        void AftInvoke(ProxyContext context, object objState);
    }
}
