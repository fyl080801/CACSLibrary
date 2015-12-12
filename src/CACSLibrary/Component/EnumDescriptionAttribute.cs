using System;
using System.Reflection;

namespace CACSLibrary.Component
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)]
    public class EnumDescriptionAttribute : Attribute
    {
        FieldInfo _field;
        string _description;

        /// <summary>
        /// 
        /// </summary>
        public EnumDescriptionAttribute()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="description"></param>
        public EnumDescriptionAttribute(string description)
        {
            if (string.IsNullOrEmpty(description))
                throw new ArgumentNullException("description");
            this._description = description;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Description
        {
            get { return this._description; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string FieldName
        {
            get { return this._field.Name; }
        }



        internal void SetFieldInfo(FieldInfo fieldInfo)
        {
            this._field = fieldInfo;
        }
    }
}
