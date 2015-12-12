using System;

namespace CACSLibrary.Data
{
    /// <summary>
    /// 实体对象基础类，主键为整数
    /// </summary>
    /// <remarks>
    /// 数据库表映射类型都必须从这里派生
    /// </remarks>
    [Serializable]
    public abstract class BaseObjectEntity // : BaseEntity<int>
    {
        /// <summary>
        /// 
        /// </summary>
        public virtual object Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as BaseObjectEntity);
        }

        private static bool IsTransient(BaseObjectEntity obj)
        {
            return obj != null && object.Equals(obj.Id, default(object));
        }

        private Type GetUnproxiedType()
        {
            return base.GetType();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual bool Equals(BaseObjectEntity other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (!IsTransient(this) &&
                !IsTransient(other) &&
                Equals(Id, other.Id))
            {
                var otherType = other.GetUnproxiedType();
                var thisType = GetUnproxiedType();
                return thisType.IsAssignableFrom(otherType) ||
                        otherType.IsAssignableFrom(thisType);
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if (Equals(Id, default(int)))
                return base.GetHashCode();
            return Id.GetHashCode();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==(BaseObjectEntity x, BaseObjectEntity y)
        {
            return object.Equals(x, y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator !=(BaseObjectEntity x, BaseObjectEntity y)
        {
            return !(x == y);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class BaseEntity<T> : BaseObjectEntity where T : struct
    {
        /// <summary>
        /// 
        /// </summary>
        new public virtual T Id
        {
            get { return base.Id == null ? default(T) : (T)base.Id; }
            set { base.Id = value; }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public abstract class BaseEntity : BaseEntity<int>
    {

    }
}
