using System;

namespace CACSLibrary.Component
{
    /// <summary>
    /// 
    /// </summary>
    public class CustomPropertyInfo
    {
        string propertyName;
        string type;

        /// <summary>
        /// 
        /// </summary>
        public CustomPropertyInfo()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propertyName"></param>
        public CustomPropertyInfo(string type, string propertyName)
        {
            this.type = type;
            this.propertyName = propertyName;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Type
        {
            get { return this.type; }
            set { this.type = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string PropertyName
        {
            get { return this.propertyName; }
            set { this.propertyName = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string FieldName
        {
            get
            {
                if (this.propertyName.Length < 1)
                {
                    return "";
                }
                return this.propertyName.Substring(0, 1).ToLower() + this.propertyName.Substring(1);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string SetPropertyMethodName
        {
            get { return "set_" + this.PropertyName; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string GetPropertyMethodName
        {
            get { return "get_" + this.PropertyName; }
        }
    }
}
