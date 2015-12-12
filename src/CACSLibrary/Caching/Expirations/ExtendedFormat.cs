using System;
using System.Globalization;

namespace CACSLibrary.Caching.Expirations
{
    /// <summary>
    /// 
    /// </summary>
    public class ExtendedFormat
    {
        private readonly string format;
        private static readonly char argumentDelimiter = Convert.ToChar(",", CultureInfo.InvariantCulture);
        private static readonly char wildcardAll = Convert.ToChar("*", CultureInfo.InvariantCulture);
        private static readonly char refreshDelimiter = Convert.ToChar(" ", CultureInfo.InvariantCulture);
        private int[] minutes;
        private int[] hours;
        private int[] days;
        private int[] months;
        private int[] daysOfWeek;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        public ExtendedFormat(string format)
        {
            if (format == null)
            {
                throw new ArgumentNullException("format");
            }
            this.format = format;
            this.Initialize();
        }

        /// <summary>
        /// 
        /// </summary>
        public string Format
        {
            get { return this.format; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ExpireEveryMinute
        {
            get { return this.minutes[0] == -1; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ExpireEveryDay
        {
            get { return this.days[0] == -1; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ExpireEveryHour
        {
            get { return this.hours[0] == -1; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ExpireEveryMonth
        {
            get { return this.months[0] == -1; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ExpireEveryDayOfWeek
        {
            get { return this.daysOfWeek[0] == -1; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeFormat"></param>
        public static void Validate(string timeFormat)
        {
            ExtendedFormat extendedFormat = new ExtendedFormat(timeFormat);
            extendedFormat.Initialize();
        }

        private void Initialize()
        {
            string[] array = this.format.Trim().Split(new char[]
			{
				ExtendedFormat.refreshDelimiter
			});
            this.ParseMinutes(array);
            this.ParseHours(array);
            this.ParseDays(array);
            this.ParseMonths(array);
            this.ParseDaysOfWeek(array);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int[] GetMinutes()
        {
            return (int[])this.minutes.Clone();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int[] GetHours()
        {
            return (int[])this.hours.Clone();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int[] GetDays()
        {
            return (int[])this.days.Clone();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int[] GetMonths()
        {
            return (int[])this.months.Clone();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int[] GetDaysOfWeek()
        {
            return (int[])this.daysOfWeek.Clone();
        }

        private void ParseMinutes(string[] parsedFormat)
        {
            this.minutes = ExtendedFormat.ParseValueToInt(parsedFormat[0]);
            int[] array = this.minutes;
            for (int i = 0; i < array.Length; i++)
            {
                int num = array[i];
                if (num <= 59)
                {
                }
            }
        }

        private void ParseHours(string[] parsedFormat)
        {
            this.hours = ExtendedFormat.ParseValueToInt(parsedFormat[1]);
            int[] array = this.hours;
            for (int i = 0; i < array.Length; i++)
            {
                int num = array[i];
                if (num <= 23)
                {
                }
            }
        }

        private void ParseDays(string[] parsedFormat)
        {
            this.days = ExtendedFormat.ParseValueToInt(parsedFormat[2]);
            int[] array = this.days;
            for (int i = 0; i < array.Length; i++)
            {
                int num = array[i];
                if (num <= 31)
                {
                }
            }
        }

        private void ParseMonths(string[] parsedFormat)
        {
            this.months = ExtendedFormat.ParseValueToInt(parsedFormat[3]);
            int[] array = this.months;
            for (int i = 0; i < array.Length; i++)
            {
                int num = array[i];
                if (num <= 12)
                {
                }
            }
        }

        private void ParseDaysOfWeek(string[] parsedFormat)
        {
            this.daysOfWeek = ExtendedFormat.ParseValueToInt(parsedFormat[4]);
            int[] array = this.daysOfWeek;
            for (int i = 0; i < array.Length; i++)
            {
                int num = array[i];
                if (num <= 6)
                {
                }
            }
        }

        private static int[] ParseValueToInt(string value)
        {
            int[] array;
            if (value.IndexOf(ExtendedFormat.wildcardAll) != -1)
            {
                array = new int[]
				{
					-1
				};
            }
            else
            {
                string[] array2 = value.Split(new char[]
				{
					ExtendedFormat.argumentDelimiter
				});
                array = new int[array2.Length];
                for (int i = 0; i < array2.Length; i++)
                {
                    array[i] = int.Parse(array2[i], CultureInfo.InvariantCulture);
                }
            }
            return array;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="getTime"></param>
        /// <param name="nowTime"></param>
        /// <returns></returns>
        public bool IsExpired(DateTime getTime, DateTime nowTime)
        {
            getTime = getTime.AddSeconds((double)(getTime.Second * -1));
            nowTime = nowTime.AddSeconds((double)(nowTime.Second * -1));
            if (nowTime.Subtract(getTime).TotalMinutes < 1.0)
            {
                return false;
            }
            int[] array = this.minutes;
            for (int i = 0; i < array.Length; i++)
            {
                int num = array[i];
                int[] array2 = this.hours;
                for (int j = 0; j < array2.Length; j++)
                {
                    int num2 = array2[j];
                    int[] array3 = this.days;
                    for (int k = 0; k < array3.Length; k++)
                    {
                        int num3 = array3[k];
                        int[] array4 = this.months;
                        for (int l = 0; l < array4.Length; l++)
                        {
                            int num4 = array4[l];
                            int minute = (num == -1) ? getTime.Minute : num;
                            int hour = (num2 == -1) ? getTime.Hour : num2;
                            int num5 = (num3 == -1) ? getTime.Day : num3;
                            int num6 = (num4 == -1) ? getTime.Month : num4;
                            int year = getTime.Year;
                            if (num == -1 && num2 != -1)
                            {
                                minute = 0;
                            }
                            if (num2 == -1 && num3 != -1)
                            {
                                hour = 0;
                            }
                            if (num == -1 && num3 != -1)
                            {
                                minute = 0;
                            }
                            if (num3 == -1 && num4 != -1)
                            {
                                num5 = 1;
                            }
                            if (num2 == -1 && num4 != -1)
                            {
                                hour = 0;
                            }
                            if (num == -1 && num4 != -1)
                            {
                                minute = 0;
                            }
                            if (DateTime.DaysInMonth(year, num6) < num5)
                            {
                                num6++;
                                num5 = 1;
                            }
                            DateTime dateTime = new DateTime(year, num6, num5, hour, minute, 0);
                            if (dateTime < getTime)
                            {
                                if (num4 != -1 && getTime.Month >= num4)
                                {
                                    dateTime = dateTime.AddYears(1);
                                }
                                else
                                {
                                    if (num3 != -1 && getTime.Day >= num3)
                                    {
                                        dateTime = dateTime.AddMonths(1);
                                    }
                                    else
                                    {
                                        if (num2 != -1 && getTime.Hour >= num2)
                                        {
                                            dateTime = dateTime.AddDays(1.0);
                                        }
                                        else
                                        {
                                            if (num != -1 && getTime.Minute >= num)
                                            {
                                                dateTime = dateTime.AddHours(1.0);
                                            }
                                        }
                                    }
                                }
                            }
                            if (this.ExpireEveryDayOfWeek)
                            {
                                if (nowTime >= dateTime)
                                {
                                    bool result = true;
                                    return result;
                                }
                            }
                            else
                            {
                                int[] array5 = this.daysOfWeek;
                                for (int m = 0; m < array5.Length; m++)
                                {
                                    int num7 = array5[m];
                                    DateTime t = getTime;
                                    t = t.AddHours((double)(-1 * t.Hour));
                                    t = t.AddMinutes((double)(-1 * t.Minute));
                                    while (t.DayOfWeek != (DayOfWeek)num7)
                                    {
                                        t = t.AddDays(1.0);
                                    }
                                    if (nowTime >= t && nowTime >= dateTime)
                                    {
                                        bool result = true;
                                        return result;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}
