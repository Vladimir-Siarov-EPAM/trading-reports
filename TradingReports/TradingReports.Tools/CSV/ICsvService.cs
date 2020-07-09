using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TradingReports.Core.BE;

namespace TradingReports.Tools.CSV
{
	public interface ICsvService
	{
		/// <summary>
		/// Writes provided trading data in CSV format to the specified stream writer.
		/// </summary>
		/// <param name="dayTradingData"></param>
		/// <param name="headers"></param>
		/// <param name="fnToLocalTime"></param>
		/// <param name="csvSettings"></param>
		/// <param name="streamWriter"></param>
		/// <returns></returns>
		Task WriteAsCsv(IEnumerable<TradingHourlyData> dayTradingData,
			string[] headers,
			Func<DateTime, DateTime> fnToLocalTime,
			CsvSettings csvSettings,
			StreamWriter streamWriter);
	}
}
