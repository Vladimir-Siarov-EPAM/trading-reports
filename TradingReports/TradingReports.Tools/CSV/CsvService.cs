using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TradingReports.Core.BE;

namespace TradingReports.Tools.CSV
{
	/// <summary>
	/// Contains helper method for CSV export.
	/// </summary>
	public class CsvService : ICsvService
	{
		/// <inheritdoc/>
		public async Task WriteAsCsv(IEnumerable<TradingHourlyData> dayTradingData,
			string[] headers,
			Func<DateTime, DateTime> fnToLocalTime,
			CsvSettings csvSettings,
			StreamWriter streamWriter)
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

			WriteHeaderLine(actualHeaders, streamWriter, csvSettings);
			foreach (var hourlyData in dayTradingData)
			{
				WriteDataLine(hourlyData, fnToLocalTime, streamWriter, csvSettings);
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
		}
	}
}
