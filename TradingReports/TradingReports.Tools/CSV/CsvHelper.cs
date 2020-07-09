using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using TradingReports.Core.BE;

namespace TradingReports.Tools.CSV
{
	public class CsvHelper
	{
		public static readonly CsvSettings DefaultCsvSettings;


		static CsvHelper()
		{
			DefaultCsvSettings = new CsvSettings
			{
				FieldSeparator = ConfigurationManager.AppSettings["CsvSettings:FieldSeparator"],
				Qualifier = ConfigurationManager.AppSettings["CsvSettings:Qualifier"],
				TimeFormat = ConfigurationManager.AppSettings["CsvSettings:TimeFormat"],
				NumberFormat = ConfigurationManager.AppSettings["CsvSettings:NumberFormat"]
			};

			// set default values
			DefaultCsvSettings.FieldSeparator = !string.IsNullOrEmpty(DefaultCsvSettings.FieldSeparator)
				? DefaultCsvSettings.FieldSeparator
				: ",";
			DefaultCsvSettings.Qualifier = DefaultCsvSettings.Qualifier ?? string.Empty;
			DefaultCsvSettings.TimeFormat = !string.IsNullOrEmpty(DefaultCsvSettings.TimeFormat)
				? DefaultCsvSettings.TimeFormat
				: "HH:mm";
			DefaultCsvSettings.NumberFormat = !string.IsNullOrEmpty(DefaultCsvSettings.NumberFormat)
				? DefaultCsvSettings.NumberFormat
				: "0.####";
		}


		/// <summary>
		///  Writes provided trading data in CSV format to the specified stream writer.
		/// </summary>
		/// <param name="dayTradingData"></param>
		/// <param name="headers"></param>
		/// <param name="fnToLocalTime"></param>
		/// <param name="streamWriter"></param>
		/// <param name="customCsvSettings"></param>
		/// <returns></returns>
		public static async Task WriteAsCsv(IEnumerable<TradingHourlyData> dayTradingData,
			string[] headers,
			Func<DateTime, DateTime> fnToLocalTime,
			StreamWriter streamWriter,
			CsvSettings customCsvSettings = null)
		{
			if (dayTradingData == null)
				return;
			if (streamWriter == null)
				throw new ArgumentNullException(nameof(streamWriter));

			string[] actualHeaders = new[]
			{
				headers != null && headers.Length > 0 ? headers[0] : "Local Time",
				headers != null && headers.Length > 1 ? headers[1] : "Volume",
			};

			WriteHeaderLine(actualHeaders, streamWriter, customCsvSettings ?? DefaultCsvSettings);
			foreach (var hourlyData in dayTradingData)
			{
				WriteDataLine(hourlyData, fnToLocalTime, streamWriter, customCsvSettings ?? DefaultCsvSettings);
			}

			await streamWriter.FlushAsync();
		}


		private static void WriteHeaderLine(string[] headers, 
			StreamWriter streamWriter, CsvSettings csvSettings)
		{
			for (int i = 0; i < headers.Length; i++)
			{
				streamWriter.Write(csvSettings.Qualifier);
				{
					streamWriter.Write(headers[i]);
				}
				streamWriter.Write(csvSettings.Qualifier);

				if (i < headers.Length - 1)
				{
					streamWriter.Write(csvSettings.FieldSeparator);
				}
			}

			streamWriter.Write(Environment.NewLine);
			//streamWriter.Flush();
		}

		private static void WriteDataLine(TradingHourlyData hourlyData,
			Func<DateTime, DateTime> fnToLocalTime,
			StreamWriter streamWriter, CsvSettings csvSettings)
		{
			streamWriter.Write(csvSettings.Qualifier);
			{
				string strDate = fnToLocalTime != null
					? fnToLocalTime(hourlyData.PeriodUtcDate).ToString(csvSettings.TimeFormat)
					: hourlyData.PeriodUtcDate.ToString(csvSettings.TimeFormat);

				streamWriter.Write(strDate);
			}
			streamWriter.Write(csvSettings.Qualifier);

			streamWriter.Write(csvSettings.FieldSeparator);

			streamWriter.Write(csvSettings.Qualifier);
			{
				streamWriter.Write(hourlyData.Volume.ToString(csvSettings.NumberFormat));
			}
			streamWriter.Write(csvSettings.Qualifier);

			streamWriter.Write(Environment.NewLine);
			//streamWriter.Flush();
		}
	}
}
