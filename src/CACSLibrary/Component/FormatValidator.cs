using System;
using System.Text.RegularExpressions;

namespace CACSLibrary.Component
{
    /// <summary>
    /// �ַ�����ʽ��֤������
    /// </summary>
	public class FormatValidator
	{

        /// <summary>
        /// �Ƿ����ļ�Ŀ¼
        /// </summary>
        /// <param name="input">����</param>
        /// <returns>���</returns>
		public static bool IsFilePath(string input)
		{
			return !string.IsNullOrEmpty(input) && Regex.IsMatch(input, "(^\\\\.|^/|^[a-zA-Z])?:?/.+(/$)?");
		}

        /// <summary>
        /// �Ƿ���С��
        /// </summary>
        /// <param name="input">����</param>
        /// <returns>���</returns>
		public static bool IsDecimal(string input)
		{
			return !string.IsNullOrEmpty(input) && Regex.IsMatch(input, "^[0-9]+(\\.[0-9]+)?$");
		}

        /// <summary>
        /// �Ƿ��Ǵ�����С��
        /// </summary>
        /// <param name="input">����</param>
        /// <returns>���</returns>
		public static bool IsDecimalSign(string input)
		{
			return !string.IsNullOrEmpty(input) && Regex.IsMatch(input, "^[+-]?[0-9]+(\\.[0-9]+)?$");
		}

        /// <summary>
        /// �Ƿ��� Email
        /// </summary>
        /// <param name="input">����</param>
        /// <returns>���</returns>
		public static bool IsEmail(string input)
		{
			return !string.IsNullOrEmpty(input) && Regex.IsMatch(input, "^\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*$");
		}

        /// <summary>
        /// �Ƿ��� IP ��ַ
        /// </summary>
        /// <param name="input">����</param>
        /// <returns>���</returns>
		public static bool IsIP(string input)
		{
			return !string.IsNullOrEmpty(input) && Regex.IsMatch(input.Trim(), "^(\\d{1,2}|1\\d\\d|2[0-4]\\d|25[0-5])\\.(\\d{1,2}|1\\d\\d|2[0-4]\\d|25[0-5])\\.(\\d{1,2}|1\\d\\d|2[0-4]\\d|25[0-5])\\.(\\d{1,2}|1\\d\\d|2[0-4]\\d|25[0-5])$");
		}

        /// <summary>
        /// �Ƿ�������
        /// </summary>
        /// <param name="input">����</param>
        /// <returns>���</returns>
		public static bool IsNumber(string input)
		{
			return !string.IsNullOrEmpty(input) && Regex.IsMatch(input, "^[0-9]+$");
		}

        /// <summary>
        /// �Ƿ��Ǵ����ŵ�����
        /// </summary>
        /// <param name="input">����</param>
        /// <returns>���</returns>
		public static bool IsNumberSign(string input)
		{
			return !string.IsNullOrEmpty(input) && Regex.IsMatch(input, "^[+-]?[0-9]+$");
		}

        /// <summary>
        /// �Ƿ��� URL
        /// </summary>
        /// <param name="input">����</param>
        /// <returns>���</returns>
		public static bool IsUrl(string input)
		{
			return !string.IsNullOrEmpty(input) && Regex.IsMatch(input, "^http(s)?://(localhost)|([\\w-]+\\.)+[\\w-]+(/[\\w- ./?%&=]*)?$", RegexOptions.IgnoreCase);
		}
	}
}
