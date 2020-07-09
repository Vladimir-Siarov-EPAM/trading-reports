using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TradingPlatform;
using TradingReports.Core.Interfaces;

namespace TradingReports.Core.DAL
{
	public class TradingDataAdapter : ITradingDataAdapter
	{
		private readonly TradingService _tradingService;


		public TradingDataAdapter()
		{
			_tradingService = new TradingService();
		}


		public IEnumerable<Trade> GetTrades(DateTime gmtDate)
		{
			return 
				_tradingService.GetTrades(gmtDate);
		}

		public Task<IEnumerable<Trade>> GetTradesAsync(DateTime gmtDate)
		{
			return 
				_tradingService.GetTradesAsync(gmtDate);
		}
	}
}
