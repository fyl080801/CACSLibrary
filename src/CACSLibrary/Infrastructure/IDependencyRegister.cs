using System;

namespace CACSLibrary.Infrastructure
{
    /// <summary>
    /// 依赖项注册
    /// </summary>
    /// <remarks>
    /// 如果在引擎初始化时想在容器中注册某个类，需要用这个接口
    /// </remarks>
    public interface IDependencyRegister
    {
        /// <summary>
        /// 优先级
        /// </summary>
        EngineLevels Level { get; }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="containerManager">容器管理</param>
        /// <param name="typeFinder">类型查找</param>
        void Register(IContainerManager containerManager, ITypeFinder typeFinder);
    }
}
