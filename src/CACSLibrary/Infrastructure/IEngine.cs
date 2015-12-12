using CACSLibrary.Configuration;
using System;

namespace CACSLibrary.Infrastructure
{
    /// <summary>
    /// ����
    /// </summary>
    /// <remarks>��������˳������еĻ�����Ϊ���������������Ͷ�������ȡ��������͵�ʵ��</remarks>
    public interface IEngine
    {
        /// <summary>
        /// ��������
        /// </summary>
        IContainerManager ContainerManager
        {
            get;
        }

        /// <summary>
        /// ��ʼ������
        /// </summary>
        /// <param name="config">����</param>
        void Initialize(CACSConfig config);

        /// <summary>
        /// ����ָ�����͵�ʵ��
        /// </summary>
        /// <typeparam name="T">����</typeparam>
        /// <returns>ʵ��</returns>
        T Resolve<T>() where T : class;

        /// <summary>
        /// ����ָ�����͵�ʵ��
        /// </summary>
        /// <param name="type">����</param>
        /// <returns>ʵ��</returns>
        object Resolve(Type type);

        /// <summary>
        /// �������͵�����ʵ��
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T[] ResolveAll<T>();
    }
}
