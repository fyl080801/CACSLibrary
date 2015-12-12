using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace CACSLibrary.Component
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GenericListTypeConverter<T> : TypeConverter
    {
        readonly TypeConverter _typeConverter;

        /// <summary>
        /// 
        /// </summary>
        public GenericListTypeConverter()
        {
            this._typeConverter = TypeDescriptor.GetConverter(typeof(T));
            if (this._typeConverter == null)
            {
                throw new InvalidOperationException("No type converter exists for type " + typeof(T).FullName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected virtual string[] GetStringArray(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                string[] array = input.Split(new char[]
				{
					','
				});
                Array.ForEach<string>(array, delegate(string s)
                {
                    s.Trim();
                });
                return array;
            }
            return new string[0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                string[] stringArray = this.GetStringArray(sourceType.ToString());
                return stringArray.Count<string>() > 0;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                string[] stringArray = this.GetStringArray((string)value);
                List<T> result = new List<T>();
                Array.ForEach<string>(stringArray, delegate(string s)
                {
                    object obj = this._typeConverter.ConvertFromInvariantString(s);
                    if (obj != null)
                    {
                        result.Add((T)((object)obj));
                    }
                });
                return result;
            }
            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                string text = string.Empty;
                if ((IList<T>)value != null)
                {
                    for (int i = 0; i < ((IList<T>)value).Count; i++)
                    {
                        string str = Convert.ToString(((IList<T>)value)[i], CultureInfo.InvariantCulture);
                        text += str;
                        if (i != ((IList<T>)value).Count - 1)
                        {
                            text += ",";
                        }
                    }
                }
                return text;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
