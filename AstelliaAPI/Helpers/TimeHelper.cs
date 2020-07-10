using System;
using System.Globalization;

namespace AstelliaAPI.Helpers
{
    public static class TimeHelper
    {
        public static string ToRfc3339String(this DateTime d)
        {
            return d.ToString("yyyy-MM-dd'T'HH:mm:ss.fffzzz", DateTimeFormatInfo.InvariantInfo);
        }

        public static DateTime UnixTimestampToDateTime(double unixTimestamp)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimestamp).ToLocalTime();
            return dateTime;
        }

        public static int CurrentUnixTimestamp()
        {
            return (int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }
    }
}