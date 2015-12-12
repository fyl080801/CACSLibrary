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
    public interface IPersistentCollection<T> : ICollection<T>, IEnumerable<T>, ICollection, IEnumerable where T : class
    {
        /// <summary>
        /// 
        /// </summary>
        Action<ICollection<T>> AfterAdd
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        Action<ICollection<T>> AfterRemove
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        Func<ICollection<T>, T, bool> BeforeAdd
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        Func<ICollection<T>, T, bool> BeforeRemove
        {
            get;
            set;
        }
    }
}
