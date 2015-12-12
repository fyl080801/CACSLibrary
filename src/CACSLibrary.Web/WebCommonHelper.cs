using CACSLibrary.Component;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Security;
using System.Web;

namespace CACSLibrary.Web
{
    /// <summary>
    /// 
    /// </summary>
    public class WebCommonHelper
    {
        private static AspNetHostingPermissionLevel? _trustLevel = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static TypeConverter GetCustomTypeConverter(Type type)
        {
            if (type == typeof(List<int>))
            {
                return new GenericListTypeConverter<int>();
            }
            if (type == typeof(List<decimal>))
            {
                return new GenericListTypeConverter<decimal>();
            }
            if (type == typeof(List<string>))
            {
                return new GenericListTypeConverter<string>();
            }
            return TypeDescriptor.GetConverter(type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static AspNetHostingPermissionLevel GetTrustLevel()
        {
            if (!_trustLevel.HasValue)
            {
                _trustLevel = AspNetHostingPermissionLevel.None;
                AspNetHostingPermissionLevel[] levelArray = new AspNetHostingPermissionLevel[] { AspNetHostingPermissionLevel.Unrestricted, AspNetHostingPermissionLevel.High, AspNetHostingPermissionLevel.Medium, AspNetHostingPermissionLevel.Low, AspNetHostingPermissionLevel.Minimal };
                foreach (AspNetHostingPermissionLevel level in levelArray)
                {
                    try
                    {
                        new AspNetHostingPermission(level).Demand();
                        _trustLevel = new AspNetHostingPermissionLevel?(level);
                        break;
                    }
                    catch (SecurityException)
                    {
                    }
                }
            }
            return _trustLevel.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T To<T>(object value)
        {
            return (T)To(value, typeof(T));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public static object To(object value, Type destinationType)
        {
            return To(value, destinationType, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="destinationType"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static object To(object value, Type destinationType, CultureInfo culture)
        {
            if (value != null)
            {
                Type type = value.GetType();
                TypeConverter customTypeConverter = GetCustomTypeConverter(destinationType);
                TypeConverter converter2 = GetCustomTypeConverter(type);
                if ((customTypeConverter != null) && customTypeConverter.CanConvertFrom(value.GetType()))
                {
                    return customTypeConverter.ConvertFrom(null, culture, value);
                }
                if ((converter2 != null) && converter2.CanConvertTo(destinationType))
                {
                    return converter2.ConvertTo(null, culture, value, destinationType);
                }
                if (destinationType.IsEnum && (value is int))
                {
                    return Enum.ToObject(destinationType, (int)value);
                }
                if (!destinationType.IsAssignableFrom(value.GetType()))
                {
                    return Convert.ChangeType(value, destinationType, culture);
                }
            }
            return value;
        }
    }
}

