using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CACSLibrary
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QueryList<T> : List<T>, IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        public QueryList(IQueryable<T> source)
        {
            base.AddRange(source.ToList<T>());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        public QueryList(IQueryable<object> source)
        {
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
            base.AddRange(source.ToList().ConvertAll<T>(entityConverter));
        }
    }
}
