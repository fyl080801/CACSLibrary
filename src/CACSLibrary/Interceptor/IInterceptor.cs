using System;

namespace CACSLibrary.Interceptor
{
    /// <summary>
    /// �������ӿ�
    /// </summary>
    public interface IInterceptor
    {
        /// <summary>
        /// ִ��˳��
        /// </summary>
        /// <remarks>
        /// <para>
        ///     �����˵����ز���ִ��ʱ�������ִ��˳��
        /// </para>
        /// <para>
        ///     ˳��ԽС�ڷ���ִ��ǰԽ��ִ�У�˳��Խ��ִ��Խ��
        /// </para>
        /// </remarks>
        int Index
        {
            get;
            set;
        }

        /// <summary>
        /// �������ô���ӿ�
        /// </summary>
        /// <returns>���ô���ӿ�</returns>
        ICallHandler BuildCallHandler();
    }
}
