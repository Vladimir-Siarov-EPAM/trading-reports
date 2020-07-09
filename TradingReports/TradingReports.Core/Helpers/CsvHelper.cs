using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using TradingReport.Core.BE;

namespace TradingReport.Core.Helpers
{
	public static class CsvHelper
	{
		public static readonly string FieldSeparator;
		public static readonly string Qualifier;
		public static readonly string TimeFormat;
		public static readonly string NumberFormat;


		static CsvHelper()
		{
			FieldSeparator = ConfigurationManager.AppSettings["CsvHelper:FieldSeparator"];
			Qualifier = ConfigurationManager.AppSettings["CsvHelper:Qualifier"];
			TimeFormat = ConfigurationManager.AppSettings["CsvHelper:TimeFormat"];
			NumberFormat = ConfigurationManager.AppSettings["CsvHelper:NumberFormat"];

			// set default values
			FieldSeparator = !string.IsNullOrEmpty(FieldSeparator) ? FieldSeparator : ",";
			Qualifier = Qualifier ?? string.Empty;
			TimeFormat = !string.IsNullOrEmpty(TimeFormat) ? TimeFormat : "HH:MM";
			NumberFormat = !string.IsNullOrEmpty(NumberFormat) ? NumberFormat : "0.####";
		}


		public static void WriteToCsv(IEnumerable<TradingHourlyData> dayTradingData,
			string[] headers,
			Func<DateTime, DateTime> fnToLocalTime,
			StreamWriter streamWriter)
		{
			if(dayTradingData == null)
				return;
			if(streamWriter == null)
				throw new ArgumentNullException(nameof(streamWriter));

			string[] actualHeaders = new []
			{
				headers != null && headers.Length > 0 ? headers[0] : "Local Time",
				headers != null && headers.Length > 1 ? headers[1] : "Volume",
			};
			
			WriteHeaderLine(actualHeaders, streamWriter);
			foreach (var hourlyData in dayTradingData)
			{
				WriteDataLine(hourlyData, fnToLocalTime, streamWriter);
			}
		}


		private static void WriteHeaderLine(string[] headers, StreamWriter streamWriter)
		{
			for (int i = 0; i < headers.Length; i++)
			{
				streamWriter.Write(Qualifier);
				{
					streamWriter.Write(headers[i]);
				}
				streamWriter.Write(Qualifier);

				if (i < headers.Length - 1)
				{
					streamWriter.Write(FieldSeparator);
				}
			}

			streamWriter.Write(Environment.NewLine);
			streamWriter.Flush();
		}

		private static void WriteDataLine(TradingHourlyData hourlyData,
			Func<DateTime, DateTime> fnToLocalTime,
			StreamWriter streamWriter)
		{
			streamWriter.Write(Qualifier);
			{
				string strDate = fnToLocalTime != null
					? fnToLocalTime(hourlyData.PeriodUtcDate).ToString(TimeFormat)
					: hourlyData.PeriodUtcDate.ToString(TimeFormat);

				streamWriter.Write(strDate);
			}
			streamWriter.Write(Qualifier);

			streamWriter.Write(FieldSeparator);

			streamWriter.Write(Qualifier);
			{
				streamWriter.Write(hourlyData.Volume.ToString(NumberFormat));
			}
			streamWriter.Write(Qualifier);

			streamWriter.Write(Environment.NewLine);
			streamWriter.Flush();
		}
	}
}
