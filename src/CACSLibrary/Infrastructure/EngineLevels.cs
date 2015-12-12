using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CACSLibrary.Infrastructure
{
    /// <summary>
    /// 优先级
    /// </summary>
    /// <remarks>
    /// 优先级的定义
    /// <note type="tip">
    /// <list class="bullet">
    ///	<listItem>
    ///	理论上程序内部执行需要优先级最高，要用 Priority
    /// </listItem>
    /// <listItem>
    /// 被其他插件依赖的越多需要的优先级越高，需要 High
    /// </listItem>
    /// <listItem>
    /// 单独的插件优先级低，需要 Low
    /// </listItem>
    /// </list>
    /// </note>
    /// </remarks>
    public enum EngineLevels
    {
        /// <summary>
        /// 最优先
        /// </summary>
        Priority = 0,

        /// <summary>
        /// 高
        /// </summary>
        High,

        /// <summary>
        /// 中
        /// </summary>
        Normal,

        /// <summary>
        /// 低
        /// </summary>
        Low
    }
}
