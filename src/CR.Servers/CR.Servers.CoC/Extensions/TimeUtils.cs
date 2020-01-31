namespace CR.Servers.CoC.Extensions
{
    using System;

    internal static class TimeUtils
    {
        public const double TickDuration = 1d / 60d * 1000d;

        private static readonly DateTime UnixTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static int UnixUtcNow
        {
            get
            {
                return (int) TimeUtils.ToUnixTimestamp(DateTime.UtcNow);
            }
        }

        public static DateTime FromUnixTimestamp(double unixTimestamp)
        {
            return TimeUtils.UnixTime.AddSeconds(unixTimestamp);
        }

        public static double ToUnixTimestamp(DateTime time)
        {
            return (time - TimeUtils.UnixTime).TotalSeconds;
        }

        public static DateTime FromJavaTimestamp(double javaTimestamp)
        {
            return TimeUtils.UnixTime.AddSeconds(javaTimestamp / 1000);
        }

        public static double ToJavaTimestamp(DateTime time)
        {
            return (time - TimeUtils.UnixTime).TotalSeconds * 1000;
        }

        public static int ToTick(TimeSpan duration)
        {
            return (int) (duration.TotalMilliseconds / TimeUtils.TickDuration);
        }

        public static int ToTick(int duration)
        {
            return (int) (duration * 1000d / TimeUtils.TickDuration);
        }

        public static double FromTick(int tick)
        {
            return tick * TimeUtils.TickDuration / 1000d;
        }

        public static DateTime FirstDayOfMonth(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, 1);
        }

        public static DateTime LastDayOfMonth(this DateTime value)
        {
            return value.FirstDayOfMonth()
                .AddMonths(1)
                .AddMinutes(-1);
        }
    }
}