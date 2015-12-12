using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CACSLibrary
{
    /// <summary>
    /// 分页列表
    /// </summary>
    /// <typeparam name="T">对象</typeparam>
    public interface IPagedList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
    {
        /// <summary>
        /// 页索引
        /// </summary>
        int PageIndex { get; }

        /// <summary>
        /// 分页大小
        /// </summary>
        int PageSize { get; }

        /// <summary>
        /// 总记录数
        /// </summary>
        int TotalCount { get; }

        /// <summary>
        /// 总页数
        /// </summary>
        int TotalPages { get; }

        /// <summary>
        /// 是否包含上一页
        /// </summary>
        bool HasPreviousPage { get; }

        /// <summary>
        /// 是否包含下一页
        /// </summary>
        bool HasNextPage { get; }
    }
}
