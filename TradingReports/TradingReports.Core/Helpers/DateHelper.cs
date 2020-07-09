using System;

namespace TradingReports.Core.Helpers
{
	/// <summary>
	/// Contains helper methods for DateTime conversion to different Time Zones.
	/// </summary>
	public static class DateHelper
	{
		public static readonly TimeZoneInfo GmtTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
		public static readonly TimeZoneInfo UtcTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("UTC");


		/// <summary>
		/// Converts specified DateTime from UTC to GMT time zone.
		/// </summary>
		/// <param name="utcDate"></param>
		/// <returns></returns>
		public static DateTime FromUtcToGmt(this DateTime utcDate)
		{
			DateTime utcDateTime
				= new DateTime(utcDate.Year, utcDate.Month, utcDate.Day, utcDate.Hour, utcDate.Minute, utcDate.Second,
						DateTimeKind.Unspecified);

			DateTime gmtDateTime = TimeZoneInfo.ConvertTime(utcDateTime, UtcTimeZoneInfo, GmtTimeZoneInfo);

			return gmtDateTime;
		}

		/// <summary>
		/// Converts specified DateTime from GMT to UTC time zone.
		/// </summary>
		/// <param name="gmtDate"></param>
		/// <returns></returns>
		public static DateTime FromGmtToUtc(this DateTime gmtDate)
		{
			DateTime gmtDateTime
				= new DateTime(gmtDate.Year, gmtDate.Month, gmtDate.Day, gmtDate.Hour, gmtDate.Minute, gmtDate.Second,
						DateTimeKind.Unspecified);

			DateTime utcDateTime = TimeZoneInfo.ConvertTime(gmtDateTime, GmtTimeZoneInfo, UtcTimeZoneInfo);

			return utcDateTime;
		}

		/// <summary>
		/// Returns actual local start time of the day, which is 23:00 (11 pm) on the previous day.
		/// </summary>
		/// <param name="localDate"></param>
		/// <returns></returns>
		public static DateTime GetLocalStartTimeOfTradingDay(this DateTime localDate)
		{
			DateTime localDateTime =
				new DateTime(localDate.Year, localDate.Month, localDate.Day, 0, 0, 0, DateTimeKind.Unspecified);

			DateTime localStartTime = localDateTime.Date.AddHours(-1.0);

			return localStartTime;
		}
	}
}
