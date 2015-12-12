using System;

namespace CACSLibrary.Infrastructure
{
    /// <summary>
    /// 组件生命周期
    /// </summary>
    public enum ComponentLifeStyle
    {
        /// <summary>
        /// 单例，一直存在
        /// </summary>
        Singleton,

        /// <summary>
        /// 临时，用完就释放
        /// </summary>
        Transient,

        /// <summary>
        /// 运行中，本次请求中有效
        /// </summary>
        LifetimeScope
    }
}
