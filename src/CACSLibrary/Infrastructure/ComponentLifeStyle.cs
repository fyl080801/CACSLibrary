using System;

namespace CACSLibrary.Infrastructure
{
    /// <summary>
    /// �����������
    /// </summary>
    public enum ComponentLifeStyle
    {
        /// <summary>
        /// ������һֱ����
        /// </summary>
        Singleton,

        /// <summary>
        /// ��ʱ��������ͷ�
        /// </summary>
        Transient,

        /// <summary>
        /// �����У�������������Ч
        /// </summary>
        LifetimeScope
    }
}
