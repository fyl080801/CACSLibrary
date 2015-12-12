using System;
using System.Threading;

namespace CACSLibrary.Component
{
    /// <summary>
    /// ������
    /// </summary>
    /// <remarks>
    /// �����̲߳���ʱ����Ψһ��
    /// </remarks>
    /// <example>
    /// <code>
    /// 
    /// public class Lock
    /// {
    ///     private static readonly ReaderWriterLockSlim _locker = new ReaderWriterLockSlim();
    ///     
    ///     public void DoLocker()
    ///     {
    ///         using(new WriteLocker(_locker))
    ///         {
    ///             //...
    ///         }
    ///     }
    /// }
    /// 
    /// </code>
    /// </example>
    public class WriteLocker : IDisposable
    {
        private readonly ReaderWriterLockSlim _locker;

        /// <summary>
        /// ��ʼһ��������
        /// </summary>
        /// <param name="locker">��</param>
        public WriteLocker(ReaderWriterLockSlim locker)
        {
            this._locker = locker;
            this._locker.EnterWriteLock();
        }

        /// <summary>
        /// �ͷŻ�����
        /// </summary>
        public void Dispose()
        {
            this._locker.ExitWriteLock();
        }
    }
}
