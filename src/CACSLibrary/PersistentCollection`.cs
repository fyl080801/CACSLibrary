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
    public class PersistentCollection<T> : IPersistentCollection<T>, ICollection<T>, IEnumerable<T>, ICollection, IEnumerable where T : class
    {
        private readonly ICollection<T> _actual;
        private Action<ICollection<T>> _afterAdd;
        private Action<ICollection<T>> _afterRemove;
        private Func<ICollection<T>, T, bool> _beforeAdd;
        private Func<ICollection<T>, T, bool> _beforeRemove;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actual"></param>
        public PersistentCollection(ICollection<T> actual)
        {
            this._actual = actual;
        }

        /// <summary>
        /// 
        /// </summary>
        public Action<ICollection<T>> AfterAdd
        {
            get { return _afterAdd ?? (_afterAdd = (ICollection<T> collection) => { }); }
            set { _afterAdd = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Action<ICollection<T>> AfterRemove
        {
            get { return _afterRemove ?? (_afterRemove = (ICollection<T> collection) => { }); }
            set { _afterRemove = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Func<ICollection<T>, T, bool> BeforeAdd
        {
            get { return _beforeAdd ?? (_beforeAdd = (ICollection<T> collection, T item) => true); }
            set { _beforeAdd = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Func<ICollection<T>, T, bool> BeforeRemove
        {
            get { return _beforeRemove ?? (_beforeRemove = (ICollection<T> collection, T item) => true); }
            set { _beforeRemove = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get { return this._actual.Count; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsReadOnly
        {
            get { return this._actual.IsReadOnly; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this._actual.GetEnumerator();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            if (this.BeforeAdd(this, item))
            {
                this._actual.Add(item);
                this.AfterAdd(this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            while (this._actual.Any<T>())
            {
                this.Remove(this._actual.First<T>());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            return this._actual.Contains(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            this._actual.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(T item)
        {
            if (this.BeforeRemove(this, item))
            {
                bool result = this._actual.Remove(item);
                this.AfterRemove(this);
                return result;
            }
            return true;
        }

        void ICollection.CopyTo(Array array, int index)
        {
            T[] array2 = new T[this._actual.Count];
            this._actual.CopyTo(array2, 0);
            Array.Copy(array2, 0, array, index, this._actual.Count);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        int ICollection.Count
        {
            get
            {
                return this._actual.Count;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
