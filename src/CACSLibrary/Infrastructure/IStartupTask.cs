using System;

namespace CACSLibrary.Infrastructure
{
    /// <summary>
    /// 启动进程
    /// </summary>
    /// <remarks>
    /// 在引擎初始化后就开始执行的程序
    /// </remarks>
    public interface IStartupTask
    {
        /// <summary>
        /// 优先级
        /// </summary>
        /// <remarks>
        /// 执行的优先级，如果是必须进程则必须是 Priority
        /// </remarks>
        EngineLevels Level { get; }

        /// <summary>
        /// 执行方法
        /// </summary>
        void Execute();
    }
}
