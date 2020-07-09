using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TradingPlatform;

namespace TradingReports.Core.Interfaces
{
	/// <summary>
	/// Wrapper of "TradingPlatform.TradingService" for DI support. 
	/// </summary>
	public interface ITradingDataAdapter
	{
		IEnumerable<Trade> GetTrades(DateTime gmtDate);

		Task<IEnumerable<Trade>> GetTradesAsync(DateTime gmtDate);
	}
}
