using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TradingPlatform;

namespace TradingReports.Core.Interfaces
{
	/// <summary>
	/// Wrapper of "TradingPlatform.TradingService". 
	/// </summary>
	public interface ITradingDataAdapter
	{
		IEnumerable<Trade> GetTrades(DateTime gmtDate);
		
		Task<IEnumerable<Trade>> GetTradesAsync(DateTime gmtDate);


		/// <summary>
		/// Calls 3th party TradingPlatform and returns hourly trading data for specified UK day.
		/// <para>Call of  the TradingPlatform is done several time in case of error</para>
		/// </summary>
		/// <param name="gmtDate"></param>
		/// <returns></returns>
		Task<IEnumerable<Trade>> GetTradesSafetyAsync(DateTime gmtDate);
	}
}
