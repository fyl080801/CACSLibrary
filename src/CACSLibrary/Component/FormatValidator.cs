using System;
using System.Text.RegularExpressions;

namespace CACSLibrary.Component
{
    /// <summary>
    /// 字符串格式验证工具类
    /// </summary>
	public class FormatValidator
	{

        /// <summary>
        /// 是否是文件目录
        /// </summary>
        /// <param name="input">输入</param>
        /// <returns>结果</returns>
		public static bool IsFilePath(string input)
		{
			return !string.IsNullOrEmpty(input) && Regex.IsMatch(input, "(^\\\\.|^/|^[a-zA-Z])?:?/.+(/$)?");
		}

        /// <summary>
        /// 是否是小数
        /// </summary>
        /// <param name="input">输入</param>
        /// <returns>结果</returns>
		public static bool IsDecimal(string input)
		{
			return !string.IsNullOrEmpty(input) && Regex.IsMatch(input, "^[0-9]+(\\.[0-9]+)?$");
		}

        /// <summary>
        /// 是否是带符号小数
        /// </summary>
        /// <param name="input">输入</param>
        /// <returns>结果</returns>
		public static bool IsDecimalSign(string input)
		{
			return !string.IsNullOrEmpty(input) && Regex.IsMatch(input, "^[+-]?[0-9]+(\\.[0-9]+)?$");
		}

        /// <summary>
        /// 是否是 Email
        /// </summary>
        /// <param name="input">输入</param>
        /// <returns>结果</returns>
		public static bool IsEmail(string input)
		{
			return !string.IsNullOrEmpty(input) && Regex.IsMatch(input, "^\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*$");
		}

        /// <summary>
        /// 是否是 IP 地址
        /// </summary>
        /// <param name="input">输入</param>
        /// <returns>结果</returns>
		public static bool IsIP(string input)
		{
			return !string.IsNullOrEmpty(input) && Regex.IsMatch(input.Trim(), "^(\\d{1,2}|1\\d\\d|2[0-4]\\d|25[0-5])\\.(\\d{1,2}|1\\d\\d|2[0-4]\\d|25[0-5])\\.(\\d{1,2}|1\\d\\d|2[0-4]\\d|25[0-5])\\.(\\d{1,2}|1\\d\\d|2[0-4]\\d|25[0-5])$");
		}

        /// <summary>
        /// 是否是数字
        /// </summary>
        /// <param name="input">输入</param>
        /// <returns>结果</returns>
		public static bool IsNumber(string input)
		{
			return !string.IsNullOrEmpty(input) && Regex.IsMatch(input, "^[0-9]+$");
		}

        /// <summary>
        /// 是否是带符号的数字
        /// </summary>
        /// <param name="input">输入</param>
        /// <returns>结果</returns>
		public static bool IsNumberSign(string input)
		{
			return !string.IsNullOrEmpty(input) && Regex.IsMatch(input, "^[+-]?[0-9]+$");
		}

        /// <summary>
        /// 是否是 URL
        /// </summary>
        /// <param name="input">输入</param>
        /// <returns>结果</returns>
		public static bool IsUrl(string input)
		{
			return !string.IsNullOrEmpty(input) && Regex.IsMatch(input, "^http(s)?://(localhost)|([\\w-]+\\.)+[\\w-]+(/[\\w- ./?%&=]*)?$", RegexOptions.IgnoreCase);
		}
	}
}
