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
        /// <param name="q"></param>
        /// <param name="property"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static IQueryBuilder<T> Between<T, P>(this IQueryBuilder<T> q, Expression<Func<T, P>> property, P from, P to)
        {
            Type type = typeof(P);
            ConstantExpression constantFrom = Expression.Constant(from);
            ConstantExpression constantTo = Expression.Constant(to);
            Expression propertyBody = QueryBuilderExtensions.GetMemberExpression<T, P>(q, property);
            Expression nonNullProperty = propertyBody;
            if (type.IsNullableType())
            {
                type = type.GetNonNullableType();
                nonNullProperty = Expression.Convert(propertyBody, type);
            }
            BinaryExpression c = Expression.GreaterThanOrEqual(nonNullProperty, constantFrom);
            BinaryExpression c2 = Expression.LessThanOrEqual(nonNullProperty, constantTo);
            BinaryExpression c3 = Expression.AndAlso(c, c2);
            q.AppendExpression(c3);
            return q;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="P"></typeparam>
        /// <param name="q"></param>
        /// <param name="property"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static IQueryBuilder<T> Between<T, P>(this IQueryBuilder<T> q, string property, P from, P to)
        {
            return q.Between(QueryBuilderExtensions.BuildMemberLambda<T, P>(property), from, to);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="q"></param>
        /// <param name="property"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static IQueryBuilder<T> Between<T>(this IQueryBuilder<T> q, Expression<Func<T, string>> property, string from, string to)
        {
            from = from.Trim();
            to = to.Trim();
            if (from != string.Empty && to != string.Empty)
            {
                Expression propertyBody = QueryBuilderExtensions.GetMemberExpression<T, string>(q, property);
                ConstantExpression constantFrom = Expression.Constant(from);
                ConstantExpression constantTo = Expression.Constant(to);
                ConstantExpression constantZero = Expression.Constant(0);
                MethodInfo compareMethod = typeof(string).GetMethod("Compare", new Type[]
                {
                    typeof(string),
                    typeof(string)
                });
                MethodCallExpression methodExp = Expression.Call(null, compareMethod, propertyBody, constantFrom);
                BinaryExpression c = Expression.GreaterThanOrEqual(methodExp, constantZero);
                MethodCallExpression methodExp2 = Expression.Call(null, compareMethod, propertyBody, constantTo);
                BinaryExpression c2 = Expression.LessThanOrEqual(methodExp2, constantZero);
                BinaryExpression c3 = Expression.AndAlso(c, c2);
                q.AppendExpression(c3);
            }
            return q;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="q"></param>
        /// <param name="property"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static IQueryBuilder<T> Between<T>(this IQueryBuilder<T> q, string property, string from, string to)
        {
            return q.Between(QueryBuilderExtensions.BuildMemberLambda<T, string>(property), from, to);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="q"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IQueryBuilder<T> LessThan<T, V>(this IQueryBuilder<T> q, Expression<Func<T, V>> property, V value)
        {
            IQueryBuilder<T> result;
            if (value == null)
            {
                result = q;
            }
            else
            {
                Expression propertyBody = QueryBuilderExtensions.GetMemberExpression<T, V>(q, property);
                Expression valueExpr = Expression.Constant(value);
                Expression methodExpr = Expression.LessThan(propertyBody, valueExpr);
                q.AppendExpression(methodExpr);
                result = q;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="q"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IQueryBuilder<T> LessThan<T, V>(this IQueryBuilder<T> q, string property, V value)
        {
            return q.LessThan(QueryBuilderExtensions.BuildMemberLambda<T, V>(property), value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="q"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IQueryBuilder<T> GreaterThan<T, V>(this IQueryBuilder<T> q, Expression<Func<T, V>> property, V value)
        {
            IQueryBuilder<T> result;
            if (value == null)
            {
                result = q;
            }
            else
            {
                Expression propertyBody = QueryBuilderExtensions.GetMemberExpression<T, V>(q, property);
                Expression valueExpr = Expression.Constant(value);
                Expression methodExpr = Expression.GreaterThan(propertyBody, valueExpr);
                q.AppendExpression(methodExpr);
                result = q;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="q"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IQueryBuilder<T> GreaterThan<T, V>(this IQueryBuilder<T> q, string property, V value)
        {
            return q.GreaterThan(QueryBuilderExtensions.BuildMemberLambda<T, V>(property), value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="q"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IQueryBuilder<T> LessThanOrEqual<T, V>(this IQueryBuilder<T> q, Expression<Func<T, V>> property, V value)
        {
            IQueryBuilder<T> result;
            if (value == null)
            {
                result = q;
            }
            else
            {
                Expression propertyBody = QueryBuilderExtensions.GetMemberExpression<T, V>(q, property);
                Expression valueExpr = Expression.Constant(value);
                Expression methodExpr = Expression.LessThanOrEqual(propertyBody, valueExpr);
                q.AppendExpression(methodExpr);
                result = q;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="q"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IQueryBuilder<T> LessThanOrEqual<T, V>(this IQueryBuilder<T> q, string property, V value)
        {
            return q.LessThanOrEqual(QueryBuilderExtensions.BuildMemberLambda<T, V>(property), value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="q"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IQueryBuilder<T> GreaterThanOrEqual<T, V>(this IQueryBuilder<T> q, Expression<Func<T, V>> property, V value)
        {
            IQueryBuilder<T> result;
            if (value == null)
            {
                result = q;
            }
            else
            {
                Expression propertyBody = QueryBuilderExtensions.GetMemberExpression<T, V>(q, property);
                Expression valueExpr = Expression.Constant(value);
                Expression methodExpr = Expression.GreaterThanOrEqual(propertyBody, valueExpr);
                q.AppendExpression(methodExpr);
                result = q;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="q"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IQueryBuilder<T> GreaterThanOrEqual<T, V>(this IQueryBuilder<T> q, string property, V value)
        {
            return q.GreaterThanOrEqual(QueryBuilderExtensions.BuildMemberLambda<T, V>(property), value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="q"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IQueryBuilder<T> Like<T>(this IQueryBuilder<T> q, Expression<Func<T, string>> property, string value)
        {
            IQueryBuilder<T> result;
            if (string.IsNullOrEmpty(value))
            {
                result = q;
            }
            else
            {
                value = value.Trim();
                Expression propertyBody = QueryBuilderExtensions.GetMemberExpression<T, string>(q, property);
                MethodCallExpression methodExpr = Expression.Call(propertyBody, typeof(string).GetMethod("Contains", new Type[]
                {
                    typeof(string)
                }), new Expression[]
                {
                    Expression.Constant(value)
                });
                q.AppendExpression(methodExpr);
                result = q;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="q"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IQueryBuilder<T> Like<T>(this IQueryBuilder<T> q, string property, string value)
        {
            return q.Like(QueryBuilderExtensions.BuildMemberLambda<T, string>(property), value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="P"></typeparam>
        /// <param name="q"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IQueryBuilder<T> Equal<T, P>(this IQueryBuilder<T> q, Expression<Func<T, P>> property, P value)
        {
            Expression right = Expression.Constant(value);
            Type type = typeof(P);
            Expression propertyBody = QueryBuilderExtensions.GetMemberExpression<T, P>(q, property);
            Expression left = propertyBody;
            if (type.IsNullableType())
            {
                right = Expression.Convert(right, type);
            }
            BinaryExpression methodExpr = Expression.Equal(left, right);
            q.AppendExpression(methodExpr);
            return q;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="P"></typeparam>
        /// <param name="q"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IQueryBuilder<T> Equal<T, P>(this IQueryBuilder<T> q, string property, P value)
        {
            return q.Equal(QueryBuilderExtensions.BuildMemberLambda<T, P>(property), value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="P"></typeparam>
        /// <param name="q"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IQueryBuilder<T> NotEqual<T, P>(this IQueryBuilder<T> q, Expression<Func<T, P>> property, P value)
        {
            Expression right = Expression.Constant(value);
            Type type = typeof(P);
            Expression propertyBody = QueryBuilderExtensions.GetMemberExpression<T, P>(q, property);
            Expression left = propertyBody;
            if (type.IsNullableType())
            {
                right = Expression.Convert(right, type);
            }
            BinaryExpression methodExpr = Expression.NotEqual(left, right);
            q.AppendExpression(methodExpr);
            return q;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="P"></typeparam>
        /// <param name="q"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IQueryBuilder<T> NotEqual<T, P>(this IQueryBuilder<T> q, string property, P value)
        {
            return q.NotEqual(QueryBuilderExtensions.BuildMemberLambda<T, P>(property), value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="P"></typeparam>
        /// <param name="q"></param>
        /// <param name="property"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static IQueryBuilder<T> In<T, P>(this IQueryBuilder<T> q, Expression<Func<T, P>> property, params P[] values)
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
                Expression propertyBody = QueryBuilderExtensions.GetMemberExpression<T, P>(q, property);
                MethodCallExpression methodExpr = Expression.Call(null, method, Expression.Constant(values), propertyBody);
                q.AppendExpression(methodExpr);
            }
            return q;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="P"></typeparam>
        /// <param name="q"></param>
        /// <param name="property"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static IQueryBuilder<T> In<T, P>(this IQueryBuilder<T> q, string property, params P[] values)
        {
            return q.In(QueryBuilderExtensions.BuildMemberLambda<T, P>(property), values);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="P"></typeparam>
        /// <param name="q"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        private static Expression GetMemberExpression<T, P>(IQueryBuilder<T> q, Expression<Func<T, P>> property)
        {
            Expression result;
            if (q.Parameters == null || q.Parameters.Length == 0)
            {
                q.Parameters = property.GetParameters<T, P>();
                result = property.Body;
            }
            else
            {
                ParameterExpressionVisitor visitor = new ParameterExpressionVisitor(q.Parameters[0]);
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
