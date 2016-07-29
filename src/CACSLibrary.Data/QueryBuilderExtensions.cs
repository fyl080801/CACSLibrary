using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CACSLibrary.Data
{
    /// <summary>
    /// 
    /// </summary>
    public static class QueryBuilderExtensions
    {
        private static MethodInfo method_Contains = (
            from m in typeof(Enumerable).GetMethods()
            where m.Name.Equals("Contains") && m.IsGenericMethod && m.GetGenericArguments().Length == 1 && m.GetParameters().Length == 2
            select m).First<MethodInfo>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="P"></typeparam>
        /// <param name="query"></param>
        /// <param name="property"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static IQueryBuilder<T> Between<T, P>(this IQueryBuilder<T> query, Expression<Func<T, P>> property, P from, P to)
        {
            Type type = typeof(P);
            ConstantExpression constantFrom = Expression.Constant(from);
            ConstantExpression constantTo = Expression.Constant(to);
            Expression propertyBody = QueryBuilderExtensions.GetMemberExpression<T, P>(query, property);
            Expression nonNullProperty = propertyBody;
            if (type.IsNullableType())
            {
                type = type.GetNonNullableType();
                nonNullProperty = Expression.Convert(propertyBody, type);
            }
            BinaryExpression up = Expression.GreaterThanOrEqual(nonNullProperty, constantFrom);
            BinaryExpression down = Expression.LessThanOrEqual(nonNullProperty, constantTo);
            BinaryExpression between = Expression.AndAlso(up, down);
            query.AppendExpression(between);
            return query;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="P"></typeparam>
        /// <param name="query"></param>
        /// <param name="property"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static IQueryBuilder<T> Between<T, P>(this IQueryBuilder<T> query, string property, P from, P to)
        {
            return query.Between(QueryBuilderExtensions.BuildMemberLambda<T, P>(property), from, to);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="property"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static IQueryBuilder<T> Between<T>(this IQueryBuilder<T> query, Expression<Func<T, string>> property, string from, string to)
        {
            from = from.Trim();
            to = to.Trim();
            if (from != string.Empty && to != string.Empty)
            {
                Expression propertyBody = QueryBuilderExtensions.GetMemberExpression<T, string>(query, property);
                ConstantExpression constantFrom = Expression.Constant(from);
                ConstantExpression constantTo = Expression.Constant(to);
                ConstantExpression constantZero = Expression.Constant(0);
                MethodInfo compareMethod = typeof(string).GetMethod("Compare", new Type[]
                {
                    typeof(string),
                    typeof(string)
                });
                MethodCallExpression methodExp = Expression.Call(null, compareMethod, propertyBody, constantFrom);
                BinaryExpression up = Expression.GreaterThanOrEqual(methodExp, constantZero);
                MethodCallExpression methodExp2 = Expression.Call(null, compareMethod, propertyBody, constantTo);
                BinaryExpression down = Expression.LessThanOrEqual(methodExp2, constantZero);
                BinaryExpression between = Expression.AndAlso(up, down);
                query.AppendExpression(between);
            }
            return query;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="property"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static IQueryBuilder<T> Between<T>(this IQueryBuilder<T> query, string property, string from, string to)
        {
            return query.Between(QueryBuilderExtensions.BuildMemberLambda<T, string>(property), from, to);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="query"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IQueryBuilder<T> LessThan<T, V>(this IQueryBuilder<T> query, Expression<Func<T, V>> property, V value)
        {
            IQueryBuilder<T> result;
            if (value == null)
            {
                result = query;
            }
            else
            {
                Expression propertyBody = QueryBuilderExtensions.GetMemberExpression<T, V>(query, property);
                Expression valueExpr = Expression.Constant(value);
                Expression methodExpr = Expression.LessThan(propertyBody, valueExpr);
                query.AppendExpression(methodExpr);
                result = query;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="query"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IQueryBuilder<T> LessThan<T, V>(this IQueryBuilder<T> query, string property, V value)
        {
            return query.LessThan(QueryBuilderExtensions.BuildMemberLambda<T, V>(property), value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="query"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IQueryBuilder<T> GreaterThan<T, V>(this IQueryBuilder<T> query, Expression<Func<T, V>> property, V value)
        {
            IQueryBuilder<T> result;
            if (value == null)
            {
                result = query;
            }
            else
            {
                Expression propertyBody = QueryBuilderExtensions.GetMemberExpression<T, V>(query, property);
                Expression valueExpr = Expression.Constant(value);
                Expression methodExpr = Expression.GreaterThan(propertyBody, valueExpr);
                query.AppendExpression(methodExpr);
                result = query;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="query"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IQueryBuilder<T> GreaterThan<T, V>(this IQueryBuilder<T> query, string property, V value)
        {
            return query.GreaterThan(QueryBuilderExtensions.BuildMemberLambda<T, V>(property), value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="query"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IQueryBuilder<T> LessThanOrEqual<T, V>(this IQueryBuilder<T> query, Expression<Func<T, V>> property, V value)
        {
            IQueryBuilder<T> result;
            if (value == null)
            {
                result = query;
            }
            else
            {
                Expression propertyBody = QueryBuilderExtensions.GetMemberExpression<T, V>(query, property);
                Expression valueExpr = Expression.Constant(value);
                Expression methodExpr = Expression.LessThanOrEqual(propertyBody, valueExpr);
                query.AppendExpression(methodExpr);
                result = query;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="query"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IQueryBuilder<T> LessThanOrEqual<T, V>(this IQueryBuilder<T> query, string property, V value)
        {
            return query.LessThanOrEqual(QueryBuilderExtensions.BuildMemberLambda<T, V>(property), value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="query"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IQueryBuilder<T> GreaterThanOrEqual<T, V>(this IQueryBuilder<T> query, Expression<Func<T, V>> property, V value)
        {
            IQueryBuilder<T> result;
            if (value == null)
            {
                result = query;
            }
            else
            {
                Expression propertyBody = QueryBuilderExtensions.GetMemberExpression<T, V>(query, property);
                Expression valueExpr = Expression.Constant(value);
                Expression methodExpr = Expression.GreaterThanOrEqual(propertyBody, valueExpr);
                query.AppendExpression(methodExpr);
                result = query;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="query"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IQueryBuilder<T> GreaterThanOrEqual<T, V>(this IQueryBuilder<T> query, string property, V value)
        {
            return query.GreaterThanOrEqual(QueryBuilderExtensions.BuildMemberLambda<T, V>(property), value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IQueryBuilder<T> Like<T>(this IQueryBuilder<T> query, Expression<Func<T, string>> property, string value)
        {
            IQueryBuilder<T> result;
            if (string.IsNullOrEmpty(value))
            {
                result = query;
            }
            else
            {
                value = value.Trim();
                Expression propertyBody = QueryBuilderExtensions.GetMemberExpression<T, string>(query, property);
                MethodCallExpression methodExpr = Expression.Call(propertyBody, typeof(string).GetMethod("Contains", new Type[]
                {
                    typeof(string)
                }), new Expression[]
                {
                    Expression.Constant(value)
                });
                query.AppendExpression(methodExpr);
                result = query;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IQueryBuilder<T> Like<T>(this IQueryBuilder<T> query, string property, string value)
        {
            return query.Like(QueryBuilderExtensions.BuildMemberLambda<T, string>(property), value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="P"></typeparam>
        /// <param name="query"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IQueryBuilder<T> Equal<T, P>(this IQueryBuilder<T> query, Expression<Func<T, P>> property, P value)
        {
            Expression right = Expression.Constant(value);
            Type type = typeof(P);
            Expression propertyBody = QueryBuilderExtensions.GetMemberExpression<T, P>(query, property);
            Expression left = propertyBody;
            if (type.IsNullableType())
            {
                right = Expression.Convert(right, type);
            }
            BinaryExpression methodExpr = Expression.Equal(left, right);
            query.AppendExpression(methodExpr);
            return query;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="P"></typeparam>
        /// <param name="query"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IQueryBuilder<T> Equal<T, P>(this IQueryBuilder<T> query, string property, P value)
        {
            return query.Equal(QueryBuilderExtensions.BuildMemberLambda<T, P>(property), value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="P"></typeparam>
        /// <param name="query"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IQueryBuilder<T> NotEqual<T, P>(this IQueryBuilder<T> query, Expression<Func<T, P>> property, P value)
        {
            Expression right = Expression.Constant(value);
            Type type = typeof(P);
            Expression propertyBody = QueryBuilderExtensions.GetMemberExpression<T, P>(query, property);
            Expression left = propertyBody;
            if (type.IsNullableType())
            {
                right = Expression.Convert(right, type);
            }
            BinaryExpression methodExpr = Expression.NotEqual(left, right);
            query.AppendExpression(methodExpr);
            return query;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="P"></typeparam>
        /// <param name="query"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IQueryBuilder<T> NotEqual<T, P>(this IQueryBuilder<T> query, string property, P value)
        {
            return query.NotEqual(QueryBuilderExtensions.BuildMemberLambda<T, P>(property), value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="P"></typeparam>
        /// <param name="query"></param>
        /// <param name="property"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static IQueryBuilder<T> In<T, P>(this IQueryBuilder<T> query, Expression<Func<T, P>> property, params P[] values)
        {
            if (values != null && values.Length > 0)
            {
                Type type = typeof(P);
                if (type.IsNullableType())
                {
                }
                MethodInfo method = QueryBuilderExtensions.method_Contains.MakeGenericMethod(new Type[]
                {
                    type
                });
                Expression propertyBody = QueryBuilderExtensions.GetMemberExpression<T, P>(query, property);
                MethodCallExpression methodExpr = Expression.Call(null, method, Expression.Constant(values), propertyBody);
                query.AppendExpression(methodExpr);
            }
            return query;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="P"></typeparam>
        /// <param name="query"></param>
        /// <param name="property"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static IQueryBuilder<T> In<T, P>(this IQueryBuilder<T> query, string property, params P[] values)
        {
            return query.In(QueryBuilderExtensions.BuildMemberLambda<T, P>(property), values);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="P"></typeparam>
        /// <param name="query"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        private static Expression GetMemberExpression<T, P>(IQueryBuilder<T> query, Expression<Func<T, P>> property)
        {
            Expression result;
            if (query.Parameters == null || query.Parameters.Length == 0)
            {
                query.Parameters = property.GetParameters<T, P>();
                result = property.Body;
            }
            else
            {
                ParameterExpressionVisitor visitor = new ParameterExpressionVisitor(query.Parameters[0]);
                Expression memberExpr = visitor.ChangeParameter(property.Body);
                result = memberExpr;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="P"></typeparam>
        /// <param name="property"></param>
        /// <returns></returns>
        private static Expression<Func<T, P>> BuildMemberLambda<T, P>(string property)
        {
            ParameterExpression paramExpr = Expression.Parameter(typeof(T));
            string[] subproperties = property.Split(new char[]
            {
                '.'
            });
            Type propertyType = typeof(T);
            Expression propertyExpr = paramExpr;
            string[] array = subproperties;
            for (int i = 0; i < array.Length; i++)
            {
                string sub = array[i];
                PropertyInfo propertyInfo = propertyType.GetProperty(sub);
                propertyExpr = Expression.Property(propertyExpr, propertyInfo);
                propertyType = propertyInfo.PropertyType;
            }
            return Expression.Lambda<Func<T, P>>(propertyExpr, new ParameterExpression[]
            {
                paramExpr
            });
        }
    }
}
