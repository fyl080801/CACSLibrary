using System;

namespace CACSLibrary.Interceptor
{
    /// <summary>
    /// ���ô���ӿ�
    /// </summary>
    /// <remarks>
    /// ���ھ���ʵ������������
    /// </remarks>
    public interface ICallHandler
    {
        /// <summary>
        /// ִ��˳��
        /// </summary>
        /// <seealso cref="CACSLibrary.Interceptor.IInterceptor.Index"/>
        int Index
        {
            get;
            set;
        }

        /// <summary>
        /// ���Դ�������쳣
        /// </summary>
        /// <remarks>
        /// �� PreInvoke ִ��ʱ��������쳣�����ִ�з���
        /// </remarks>
        bool IgnoreCallHandlerException
        {
            get;
        }

        /// <summary>
        /// ���Է����쳣
        /// </summary>
        /// <remarks>
        /// ������ִ��ʱ�����쳣�������쳣����ִ��AftInvoke
        /// </remarks>
        bool IgnoreMethodException
        {
            get;
        }

        /// <summary>
        /// �ڷ���ִ��ǰִ�еĲ���
        /// </summary>
        /// <param name="context">����������</param>
        /// <returns>����һ��������������ִ��ʱ��Ҫ����Ķ���</returns>
        object PreInvoke(ProxyContext context);

        /// <summary>
        /// �ڷ���ִ�к�ִ�еĲ���
        /// </summary>
        /// <param name="context">����������</param>
        /// <param name="objState">PreInvoke �й������</param>
        void AftInvoke(ProxyContext context, object objState);
    }
}
