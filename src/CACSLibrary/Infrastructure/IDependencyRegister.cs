using System;

namespace CACSLibrary.Infrastructure
{
    /// <summary>
    /// ������ע��
    /// </summary>
    /// <remarks>
    /// ����������ʼ��ʱ����������ע��ĳ���࣬��Ҫ������ӿ�
    /// </remarks>
    public interface IDependencyRegister
    {
        /// <summary>
        /// ���ȼ�
        /// </summary>
        EngineLevels Level { get; }

        /// <summary>
        /// ע��
        /// </summary>
        /// <param name="containerManager">��������</param>
        /// <param name="typeFinder">���Ͳ���</param>
        void Register(IContainerManager containerManager, ITypeFinder typeFinder);
    }
}
