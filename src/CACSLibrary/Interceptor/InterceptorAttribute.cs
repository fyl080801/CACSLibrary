using System;

namespace CACSLibrary.Interceptor
{
    /// <summary>
    /// 拦截器特性
    /// </summary>
    /// <remarks>
    /// 创建一个拦截器并作用到方法上使方法执行时执行拦截器操作
    /// </remarks>
    /// <example>
    /// <para>需要先创建一个拦截器处理接口</para>
    /// <code language="cs">
    /// <![CDATA[
    /// 
    /// public class CatchExceptionCallHandler : ICallHandler
    /// {
    ///     public void AftInvoke(ProxyContext context, object objState)
    ///     {
    ///         if (context.Response.Exception != null)
    ///         {
    ///             throw new FaultException<string>(context.Response.Exception.Message);
    ///         }
    ///     }
    ///
    ///     public bool IgnoreCallHandlerException
    ///     {
    ///         get { return true; }
    ///     }
    ///
    ///     public bool IgnoreMethodException
    ///     {
    ///         get { return true; }
    ///     }
    ///
    ///     public int Index
    ///     {
    ///         get;
    ///         set;
    ///     }
    ///
    ///     public object PreInvoke(ProxyContext context)
    ///     {
    ///         return null;
    ///     }
    /// }
    /// 
    /// ]]>
    /// </code>
    /// <para>创建一个拦截器特性接口，并返回拦截器处理接口</para>
    /// <code language="cs">
    /// <![CDATA[
    /// 
    /// public class CatchExceptionAttribute : InterceptorAttribute
    /// {
    ///     public override ICallHandler BuildCallHandler()
    ///     {
    ///         return new CatchExceptionCallHandler();
    ///     }
    /// }
    /// 
    /// ]]>
    /// </code>
    /// <para>将拦截器特性作用到方法上</para>
    /// <code language="cs">
    /// 
    /// [ServiceContract]
    /// public interface IMapService
    /// {
    ///     [FaultContract(typeof(string)), CatchException]
    ///     Restriction[] GetRestrictions();
    /// }
    /// 
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Interface, Inherited = true)]
    public abstract class InterceptorAttribute : Attribute, IInterceptor
    {
        /// <summary>
        /// 执行顺序
        /// </summary>
        /// <seealso cref="CACSLibrary.Interceptor.IInterceptor.Index"/>
        public int Index
        {
            get;
            set;
        }

        public ICallHandler ICallHandler
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        /// <summary>
        /// 创建调用处理接口
        /// </summary>
        /// <returns>调用处理接口</returns>
        public abstract ICallHandler BuildCallHandler();
    }
}
