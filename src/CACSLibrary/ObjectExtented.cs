namespace CACSLibrary
{
    /// <summary>
    /// Object 扩展方法
    /// </summary>
    public static class ObjectExtented
    {
        /// <summary>
        /// 转换成 Boolean
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>Boolean 对象</returns>
        public static bool ToBoolean(this object obj)
        {
            bool result = false;
            bool.TryParse(obj.ToString(), out result);
            return result;
        }

        /// <summary>
        /// 转换成 Boolean
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="def">默认值</param>
        /// <returns>Boolean 对象</returns>
        public static bool ToBoolean(this object obj, bool def)
        {
            if (obj == null)
            {
                return def;
            }
            bool result;
            if (!bool.TryParse(obj.ToString(), out result))
            {
                return def;
            }
            return result;
        }

        /// <summary>
        /// 转换成整型
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>整形对象</returns>
        public static int ToInteger(this object obj)
        {
            int result;
            int.TryParse(obj.ToString(), out result);
            return result;
        }

        /// <summary>
        /// 转换成整型
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="def">默认值</param>
        /// <returns>整型对象</returns>
        public static int ToInteger(this object obj, int def)
        {
            if (obj == null)
            {
                return def;
            }
            int result;
            if (!int.TryParse(obj.ToString(), out result))
            {
                return def;
            }
            return result;
        }

        /// <summary>
        /// 转换成长整型
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>长整型</returns>
        public static long ToLong(this object obj)
        {
            long result;
            long.TryParse(obj.ToString(), out result);
            return result;
        }

        /// <summary>
        /// 转换成长整型
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="def">默认值</param>
        /// <returns>长整型</returns>
        public static long ToLong(this object obj, long def)
        {
            if (obj == null)
            {
                return def;
            }
            long result;
            if (!long.TryParse(obj.ToString(), out result))
            {
                return def;
            }
            return result;
        }
    }
}
