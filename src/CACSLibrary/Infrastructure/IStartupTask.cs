using System;

namespace CACSLibrary.Infrastructure
{
    /// <summary>
    /// ��������
    /// </summary>
    /// <remarks>
    /// �������ʼ����Ϳ�ʼִ�еĳ���
    /// </remarks>
    public interface IStartupTask
    {
        /// <summary>
        /// ���ȼ�
        /// </summary>
        /// <remarks>
        /// ִ�е����ȼ�������Ǳ������������� Priority
        /// </remarks>
        EngineLevels Level { get; }

        /// <summary>
        /// ִ�з���
        /// </summary>
        void Execute();
    }
}
