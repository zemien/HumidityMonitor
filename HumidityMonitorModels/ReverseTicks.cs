using System;
using NodaTime;

namespace HumidityMonitorModels
{
    public static class ReverseTicks
    {
        public static string ToReverseTicks(this DateTime dateTime)
        {
            return (DateTime.MaxValue.Ticks - dateTime.Ticks).ToString("d19");
        }

        public static DateTime ToDateTime(string reverseTicks)
        {
            if (long.TryParse(reverseTicks, out var ticks))
            {
                return new DateTime(DateTime.MaxValue.Ticks - ticks, DateTimeKind.Utc);
            }

            return DateTime.MinValue;
        }

        public static ZonedDateTime ToZonedDateTime(string reverseTicks, DateTimeZone dateTimeZone)
        {
            var dateTime = ToDateTime(reverseTicks);

            var zonedDateTime = ZonedDateTime.FromDateTimeOffset(new DateTimeOffset(dateTime, TimeSpan.Zero));

            return zonedDateTime.ToInstant().InZone(dateTimeZone);
        }
    }
}
