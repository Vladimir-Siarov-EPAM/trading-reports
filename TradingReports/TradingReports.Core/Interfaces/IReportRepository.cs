using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TradingReports.Core.BE;

namespace TradingReports.Core.Interfaces
{
	public interface IReportRepository
	{
		/// <summary>
		/// Returns hourly trading data for specified UK day.
		/// </summary>
		/// <param name="utcDate"></param>
		/// <returns></returns>
		Task<IEnumerable<TradingHourlyData>> GetDayTradingDataForUkAsync(DateTime utcDate);
	}
}
