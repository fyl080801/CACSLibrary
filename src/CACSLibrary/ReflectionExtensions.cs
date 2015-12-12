using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace CACSLibrary
{
    /// <summary>
    /// 
    /// </summary>
    internal static class ReflectionExtensions
    {
        /// <summary>
        /// 获取类型构造函数
        /// </summary>
        /// <typeparam name="TDeclaring">类型</typeparam>
        /// <param name="constructorCallExpression"></param>
        /// <returns></returns>
        public static ConstructorInfo GetConstructor<TDeclaring>(Expression<Func<TDeclaring>> constructorCallExpression)
        {
            if (constructorCallExpression == null)
            {
                throw new ArgumentNullException("constructorCallExpression");
            }
            NewExpression newExpression = constructorCallExpression.Body as NewExpression;
            if (newExpression == null)
            {
                throw new ArgumentException("callExpression");
            }
            return newExpression.Constructor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static T GetCustomAttribute<T>(this MemberInfo element, bool inherit) where T : Attribute
        {
            return (T)((object)Attribute.GetCustomAttribute(element, typeof(T), inherit));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TDeclaring"></typeparam>
        /// <param name="methodCallExpression"></param>
        /// <returns></returns>
        public static MethodInfo GetMethod<TDeclaring>(Expression<Action<TDeclaring>> methodCallExpression)
        {
            if (methodCallExpression == null)
            {
                throw new ArgumentNullException("methodCallExpression");
            }
            MethodCallExpression methodCallExpression2 = methodCallExpression.Body as MethodCallExpression;
            if (methodCallExpression2 == null)
            {
                throw new ArgumentException("callExpression");
            }
            return methodCallExpression2.Method;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TDeclaring"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="propertyAccessor"></param>
        /// <returns></returns>
        public static PropertyInfo GetProperty<TDeclaring, TProperty>(Expression<Func<TDeclaring, TProperty>> propertyAccessor)
        {
            if (propertyAccessor == null)
            {
                throw new ArgumentNullException("propertyAccessor");
            }
            MemberExpression memberExpression = propertyAccessor.Body as MemberExpression;
            if (memberExpression == null || !(memberExpression.Member is PropertyInfo))
            {
                throw new ArgumentException("mex");
            }
            return (PropertyInfo)memberExpression.Member;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pi"></param>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static bool TryGetDeclaringProperty(this ParameterInfo pi, out PropertyInfo prop)
        {
            MethodInfo methodInfo = pi.Member as MethodInfo;
            if (methodInfo != null && methodInfo.IsSpecialName && methodInfo.Name.StartsWith("set_", StringComparison.Ordinal) && methodInfo.DeclaringType != null)
            {
                prop = methodInfo.DeclaringType.GetProperty(methodInfo.Name.Substring(4));
                return true;
            }
            prop = null;
            return false;
        }
    }
}
