using System;
using System.Linq.Expressions;

namespace CACSLibrary.Data
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IQueryBuilder<T>
    {
        /// <summary>
        /// 
        /// </summary>
        Expression<Func<T, bool>> Expression { get; }

        /// <summary>
        /// 
        /// </summary>
        ParameterExpression[] Parameters { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        void AppendExpression(Expression expression);
    }
}
