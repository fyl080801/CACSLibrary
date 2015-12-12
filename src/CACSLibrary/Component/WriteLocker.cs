using System;
using System.Threading;

namespace CACSLibrary.Component
{
    /// <summary>
    /// 互斥锁
    /// </summary>
    /// <remarks>
    /// 处理线程操作时对象唯一性
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
        /// 开始一个互斥锁
        /// </summary>
        /// <param name="locker">锁</param>
        public WriteLocker(ReaderWriterLockSlim locker)
        {
            this._locker = locker;
            this._locker.EnterWriteLock();
        }

        /// <summary>
        /// 释放互斥锁
        /// </summary>
        public void Dispose()
        {
            this._locker.ExitWriteLock();
        }
    }
}
