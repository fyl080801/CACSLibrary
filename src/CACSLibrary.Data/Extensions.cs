using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CACSLibrary.Data
{
    /// <summary>
    /// 
    /// </summary>
	public static class Extensions
	{
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="action"></param>
		public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
		{
			foreach (T element in source)
			{
				action(element);
			}
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
		public static Type GetNonNullableType(this Type type)
		{
			Type result;
			if (type.IsNullableType())
			{
				result = type.GetGenericArguments()[0];
			}
			else
			{
				result = type;
			}
			return result;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
		public static bool IsNullableType(this Type type)
		{
			return type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="expr"></param>
        /// <returns></returns>
		public static ParameterExpression[] GetParameters<T, S>(this Expression<Func<T, S>> expr)
		{
			return expr.Parameters.ToArray<ParameterExpression>();
		}
	}
}
