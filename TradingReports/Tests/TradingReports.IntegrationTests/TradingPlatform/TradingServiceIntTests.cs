using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TradingReports.Core.DAL;
using TradingReports.Core.Repositories;

namespace TradingReports.IntegrationTests.TradingPlatform
{
	[TestFixture]
	public class TradingServiceIntTests
	{
		[Test]
		public async Task TradingDataAdapterIntegrationTest()
		{
			// Check base integration with TradingPlatform.TradingService

			// arrange
			TradingDataAdapter tradingDataAdapter = new TradingDataAdapter();

			// act
			var trades = await tradingDataAdapter.GetTradesSafetyAsync(DateTime.Now);

			// assert
			trades.Should().NotBeNull();
			foreach (var trade in trades)
			{
				trade.Periods.Should().NotBeNull();
				trade.Periods.Length.Should().BeGreaterOrEqualTo(23);
			}
		}

		[Test]
		public async Task ReportRepositoryIntegrationTest()
		{
			// Check that we correctly pass report DateTime to the TradingPlatform.TradingService
			// Assumed that service works correctly with Summer / Winter UK time.

			// arrange
			var utcDateOfTransitionToSummerTime = new DateTime(2020, 3, 29, 1, 0, 0);
			var utcDateOfTransitionToWinterTime = new DateTime(2019, 10, 27, 2, 0, 0);
			var reportRepository = new ReportRepository(new TradingDataAdapter());
			

			// act 1
			var traidsOneDayBefore =
				await reportRepository.GetDayTradingDataForUkAsync(utcDateOfTransitionToSummerTime.AddDays(-1));
			var traids = await reportRepository.GetDayTradingDataForUkAsync(utcDateOfTransitionToSummerTime);
			var traidsOneDayAfter =
				await reportRepository.GetDayTradingDataForUkAsync(utcDateOfTransitionToSummerTime.AddDays(+1));

			// assert 1
			traidsOneDayBefore.Count().Should().Be(24);
			traids.Count().Should().Be(23);
			traidsOneDayAfter.Count().Should().Be(24);


			// act 2
			traidsOneDayBefore =
				await reportRepository.GetDayTradingDataForUkAsync(utcDateOfTransitionToWinterTime.AddDays(-1));
			traids = await reportRepository.GetDayTradingDataForUkAsync(utcDateOfTransitionToWinterTime);
			traidsOneDayAfter =
				await reportRepository.GetDayTradingDataForUkAsync(utcDateOfTransitionToWinterTime.AddDays(+1));

			// assert 1
			traidsOneDayBefore.Count().Should().Be(24);
			traids.Count().Should().Be(25);
			traidsOneDayAfter.Count().Should().Be(24);
		}
	}
}
