using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CACSLibrary.Data
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class QueryBuilder<T> : IQueryBuilder<T>
	{
		IList<Expression> _expressions;

		/// <summary>
		/// 
		/// </summary>
		public QueryBuilder()
		{
			_expressions = new List<Expression>();
		}

		/// <summary>
		/// 
		/// </summary>
		public Expression<Func<T, bool>> Expression
		{
			get { return this.RetrieveExpression(); }
		}

		/// <summary>
		/// 
		/// </summary>
		public ParameterExpression[] Parameters { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="expression"></param>
		public void AppendExpression(Expression expression)
		{
			this._expressions.Add(expression);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected virtual Expression<Func<T, bool>> RetrieveExpression()
		{
			Expression<Func<T, bool>> result;
			if (this._expressions.Count == 0)
			{
				result = ((T f) => true);
			}
			else
			{
				Expression expression = this._expressions.Aggregate((Expression e1, Expression e2) => System.Linq.Expressions.Expression.AndAlso(e1, e2));
				result = System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(expression, this.Parameters);
			}
			return result;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public static class QueryBuilder
	{
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static IQueryBuilder<T> Create<T>()
		{
			return new QueryBuilder<T>();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="sortExpression"></param>
		/// <param name="sortAsc"></param>
		/// <returns></returns>
		public static IQueryable<T> DataSorting<T>(IQueryable<T> source, string sortExpression, bool sortAsc)
		{
			string sortingDir = string.Empty;
			if (sortAsc)
			{
				sortingDir = "OrderBy";
			}
			else
			{
				sortingDir = "OrderByDescending";
			}
			ParameterExpression paramExpr = Expression.Parameter(typeof(T));
			string[] sortExpressions = sortExpression.Split(new char[]
			{
				'.'
			});
			Type propertyType = typeof(T);
			Expression propertyExpr = paramExpr;
			List<Type> types = new List<Type>();
			types.Add(propertyType);
			string[] array = sortExpressions;
			for (int i = 0; i < array.Length; i++)
			{
				string subsortExpression = array[i];
				PropertyInfo propertyInfo = propertyType.GetProperty(subsortExpression);
				propertyExpr = Expression.Property(propertyExpr, propertyInfo);
				propertyType = propertyInfo.PropertyType;
			}
			types.Add(propertyType);
			LambdaExpression lambdaExpr = Expression.Lambda(propertyExpr, new ParameterExpression[]
			{
				paramExpr
			});
			Expression expr = Expression.Call(typeof(Queryable), sortingDir, types.ToArray(), new Expression[]
			{
				source.Expression,
				lambdaExpr
			});
			return source.AsQueryable<T>().Provider.CreateQuery<T>(expr);
		}
	}
}
