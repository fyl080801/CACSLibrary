using System;
using System.Collections;
using System.Reflection;

namespace CACSLibrary.Component
{
    /// <summary>
    /// 
    /// </summary>
    public static class EnumUtility
    {
        static Hashtable cachedEnum = new Hashtable();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static string GetEnumDescription(Type enumType)
        {
            EnumDescriptionAttribute[] array = (EnumDescriptionAttribute[])enumType.GetCustomAttributes(typeof(EnumDescriptionAttribute), false);
            if (array.Length != 1)
            {
                return string.Empty;
            }
            return array[0].Description;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string GetFieldText(object enumValue)
        {
            EnumDescriptionAttribute[] fields = EnumUtility.GetFields(enumValue.GetType());
            EnumDescriptionAttribute[] array = fields;
            for (int i = 0; i < array.Length; i++)
            {
                EnumDescriptionAttribute enumDescriptionAttribute = array[i];
                if (enumDescriptionAttribute.FieldName == enumValue.ToString())
                {
                    return enumDescriptionAttribute.Description;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static EnumDescriptionAttribute[] GetFields(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("enumType", "不是枚举");
            }
            if (!EnumUtility.cachedEnum.Contains(enumType.FullName))
            {
                FieldInfo[] fields = enumType.GetFields();
                ArrayList arrayList = new ArrayList();
                FieldInfo[] array = fields;
                for (int i = 0; i < array.Length; i++)
                {
                    FieldInfo fieldInfo = array[i];
                    object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(EnumDescriptionAttribute), true);
                    if (customAttributes.Length == 1)
                    {
                        ((EnumDescriptionAttribute)customAttributes[0]).SetFieldInfo(fieldInfo);
                        arrayList.Add(customAttributes[0]);
                    }
                }
                EnumUtility.cachedEnum.Add(enumType.FullName, (EnumDescriptionAttribute[])arrayList.ToArray(typeof(EnumDescriptionAttribute)));
            }
            return (EnumDescriptionAttribute[])EnumUtility.cachedEnum[enumType.FullName];
        }
    }
}
