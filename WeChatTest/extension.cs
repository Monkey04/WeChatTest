using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeChatTest
{
    public static class extension
    {
        /// <summary>
        /// 时间戳转DateTime
        /// </summary>
        /// <param name="target"></param>
        /// <param name="timeStamp">时间戳（秒）</param>
        /// <returns></returns>
        public static DateTime TimeStampToDateTime(this long timeStamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, new DateTime().Kind).AddSeconds(timeStamp);
        }

        public static long DateTimeToTimeStamp(this DateTime target)
        {
            DateTime beginTime = new DateTime(1970, 1, 1, 0, 0, 0, target.Kind);
            return Convert.ToInt64((target - beginTime).TotalSeconds);
        }

        public static long StringToLong(this string target)
        {
            long result = 0;
            long.TryParse(target, out result);
            return result;
        }

        public static bool IsEmpty(this string target)
        {
            return String.IsNullOrEmpty(target);
        }

        public static bool IsNotEmpty(this string target)
        {
            return !String.IsNullOrEmpty(target);
        }
    }
}