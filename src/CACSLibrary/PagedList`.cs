using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CACSLibrary
{
    /// <summary>
    /// 分页列表
    /// </summary>
    /// <typeparam name="T">对象</typeparam>
    public class PagedList<T> : List<T>, IPagedList<T>, IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
    {
        /// <summary>
        /// 当前页索引
        /// </summary>
        public int PageIndex
        {
            get;
            protected set;
        }

        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize
        {
            get;
            protected set;
        }

        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalCount
        {
            get;
            protected set;
        }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages
        {
            get;
            protected set;
        }

        /// <summary>
        /// 是否有前一页
        /// </summary>
        public bool HasPreviousPage
        {
            get { return this.PageIndex > 0; }
        }

        /// <summary>
        /// 是否有后一页
        /// </summary>
        public bool HasNextPage
        {
            get
            {
                return this.PageIndex + 1 < this.TotalPages;
            }
        }

        /// <summary>
        /// 初始化分页列表，并转换分页对象
        /// </summary>
        /// <param name="source">源</param>
        /// <param name="pageIndex">分页索引</param>
        /// <param name="pageSize">分页大小</param>
        public PagedList(IQueryable<object> source, int pageIndex, int pageSize)
        {
            int num = source.Count();
            this.TotalCount = num;
            this.TotalPages = num / pageSize;
            if (num % pageSize > 0)
            {
                this.TotalPages++;
            }
            this.PageSize = pageSize;
            this.PageIndex = pageIndex;
            Converter<object, T> entityConverter = new Converter<object, T>(item =>
            {
                Type objectType = item.GetType();
                Type entityType = typeof(T);
                T entity = (T)Activator.CreateInstance(entityType);
                var properties = entityType.GetProperties();
                foreach (var propertyInfo in properties)
                {
                    var objectProperty = objectType.GetProperty(propertyInfo.Name);
                    if (objectProperty != null)
                    {
                        object value = objectProperty.GetValue(item, null);
                        propertyInfo.SetValue(entity, value, null);
                    }
                }
                return entity;
            });
            base.AddRange(source.Skip(pageIndex * pageSize).Take(pageSize).ToList().ConvertAll<T>(entityConverter));
        }

        /// <summary>
        /// 使用查询对象初始化分页列表
        /// </summary>
        /// <param name="source">源</param>
        /// <param name="pageIndex">分页索引</param>
        /// <param name="pageSize">分页大小</param>
        public PagedList(IQueryable<T> source, int pageIndex, int pageSize)
        {
            int num = source.Count<T>();
            this.TotalCount = num;
            this.TotalPages = num / pageSize;
            if (num % pageSize > 0)
            {
                this.TotalPages++;
            }
            this.PageSize = pageSize;
            this.PageIndex = pageIndex;
            base.AddRange(source.Skip(pageIndex * pageSize).Take(pageSize).ToList<T>());
        }

        /// <summary>
        /// 使用列表初始化分页列表
        /// </summary>
        /// <param name="source">源</param>
        /// <param name="pageIndex">分页索引</param>
        /// <param name="pageSize">分页大小</param>
        public PagedList(IList<T> source, int pageIndex, int pageSize)
        {
            this.TotalCount = source.Count<T>();
            this.TotalPages = this.TotalCount / pageSize;
            if (this.TotalCount % pageSize > 0)
            {
                this.TotalPages++;
            }
            this.PageSize = pageSize;
            this.PageIndex = pageIndex;
            base.AddRange(source.Skip(pageIndex * pageSize).Take(pageSize).ToList<T>());
        }

        /// <summary>
        /// 使用枚举列表初始化分页列表
        /// </summary>
        /// <param name="source">源</param>
        /// <param name="pageIndex">分页索引</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="totalCount">总记录数</param>
        public PagedList(IEnumerable<T> source, int pageIndex, int pageSize, int totalCount)
        {
            this.TotalCount = totalCount;
            this.TotalPages = this.TotalCount / pageSize;
            if (this.TotalCount % pageSize > 0)
            {
                this.TotalPages++;
            }
            this.PageSize = pageSize;
            this.PageIndex = pageIndex;
            base.AddRange(source);
        }
    }
}
