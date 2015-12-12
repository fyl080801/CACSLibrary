using System;
using System.Linq;

namespace CACSLibrary.Data
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T> where T : BaseObjectEntity
    {
        /// <summary>
        /// 
        /// </summary>
        IQueryable<T> Table { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T GetById(object id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        void Insert(T entity);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        void Insert(params T[] entities);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        void Update(T entity);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        void Update(params T[] entities);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        void Delete(T entity);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        void Delete(params T[] entities);
    }
}
