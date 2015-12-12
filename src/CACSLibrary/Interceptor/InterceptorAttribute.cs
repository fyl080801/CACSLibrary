using System;

namespace CACSLibrary.Interceptor
{
    /// <summary>
    /// ����������
    /// </summary>
    /// <remarks>
    /// ����һ�������������õ�������ʹ����ִ��ʱִ������������
    /// </remarks>
    /// <example>
    /// <para>��Ҫ�ȴ���һ������������ӿ�</para>
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
    /// <para>����һ�����������Խӿڣ�����������������ӿ�</para>
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
    /// <para>���������������õ�������</para>
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
        /// ִ��˳��
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
        /// �������ô���ӿ�
        /// </summary>
        /// <returns>���ô���ӿ�</returns>
        public abstract ICallHandler BuildCallHandler();
    }
}
