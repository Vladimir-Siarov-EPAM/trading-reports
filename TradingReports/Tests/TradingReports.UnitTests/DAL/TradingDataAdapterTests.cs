using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TradingPlatform;
using TradingReports.Core.DAL;

namespace TradingReports.UnitTests.DAL
{
	[TestFixture]
	public class TradingDataAdapterTests
	{
		[Test]
		public async Task GetTradesSafetyAsync()
		{
			#region arrange

			int attemptCnt = 0;
			int successfulAttemptNumber = 4;

			Func<DateTime, Task<IEnumerable<Trade>>> fnGetTradeAsync = (gmtDate) =>
			{
				attemptCnt++;

				if (attemptCnt < successfulAttemptNumber)
					throw new Exception("Test exception");

				Trade[] tradeArray = new[] { Trade.Create(gmtDate, 24) };

				return
					Task.FromResult((IEnumerable<Trade>)tradeArray);
			};
			
			var tradingDataAdapter = new TradingDataAdapter();

			#endregion 

			// act
			var trades = await tradingDataAdapter.GetTradesSafetyAsync(DateTime.Now, fnGetTradeAsync);

			// assert
			trades.Should().NotBeNull();
			attemptCnt.Should().Be(successfulAttemptNumber);
		}
	}
}
