using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Logging;
using TradingPlatform;
using TradingReports.Core.Interfaces;

namespace TradingReports.Core.DAL
{
	public class TradingDataAdapter : ITradingDataAdapter
	{
		private readonly ILog _logger = LogManager.GetLogger(typeof(TradingDataAdapter));
		private readonly TradingService _tradingService;
		private readonly int[] _delaysBetweenAttempts = new[] { 1, 2, 3, 5, 8, 13, 21, 34, 55, 89 };


		public TradingDataAdapter()
		{
			_tradingService = new TradingService();
		}

		#region ITradingDataAdapter

		public IEnumerable<Trade> GetTrades(DateTime gmtDate)
		{
			return 
				_tradingService.GetTrades(gmtDate);
		}
		
		public async Task<IEnumerable<Trade>> GetTradesAsync(DateTime gmtDate)
		{
			return 
				await _tradingService.GetTradesAsync(gmtDate);
		}


		/// <inheritdoc/>
		public async Task<IEnumerable<Trade>> GetTradesSafetyAsync(DateTime gmtDate)
		{
			return
				await this.GetTradesSafetyAsync(gmtDate, _tradingService.GetTradesAsync);
		}
		
		#endregion


		// internal - for test support
		internal async Task<IEnumerable<Trade>> GetTradesSafetyAsync(DateTime gmtDate,
			Func<DateTime, Task<IEnumerable<Trade>>> fnGetTradeAsync) // for test support
		{
			for (int i = 0; i <= _delaysBetweenAttempts.Length; i++)
			{
				try
				{
					return
						await fnGetTradeAsync(gmtDate);
				}
				catch (Exception ex)
				{
					_logger.Warn("Error has occurred during call to TradingService", ex);

					if (i == _delaysBetweenAttempts.Length)
						break;

					await Task.Delay(TimeSpan.FromSeconds(_delaysBetweenAttempts[i]));
				}
			}

			throw new OperationCanceledException(
				$"TradingService didn't return data after {_delaysBetweenAttempts.Length} attempts");
		}
	}
}
