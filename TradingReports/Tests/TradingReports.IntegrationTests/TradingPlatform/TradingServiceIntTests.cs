using System;
using System.Collections.Generic;
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

		#region ReportRepositoryIntegrationTest

		[TestCaseSource(nameof(ReportRepositoryIntegrationTest_Source))]
		public async Task ReportRepositoryIntegrationTest(DateTime utcReportDate, int expectedPeriodCnt)
		{
			// Check that we correctly pass report DateTime to the TradingPlatform.TradingService
			// Assumed that service works correctly with Summer / Winter UK time.

			// arrange
			var reportRepository = new ReportRepository(new TradingDataAdapter());

			// act 
			var trades = await reportRepository.GetDayTradingDataForUkAsync(utcReportDate);

			// assert
			trades.Count().Should().Be(expectedPeriodCnt);
		}

		private static IEnumerable<TestCaseData> ReportRepositoryIntegrationTest_Source()
		{
			var utcDateOfTransitionToSummerTime = new DateTime(2020, 3, 29, 1, 0, 0);
			var utcDateOfTransitionToWinterTime = new DateTime(2019, 10, 27, 2, 0, 0);

			// TestCaseData: { utcReportDate, expectedTradingPeriodCnt }
			yield return new TestCaseData(utcDateOfTransitionToSummerTime.AddDays(-1), 24);
			yield return new TestCaseData(utcDateOfTransitionToSummerTime, 23);
			yield return new TestCaseData(utcDateOfTransitionToSummerTime.AddDays(+1), 24);

			yield return new TestCaseData(utcDateOfTransitionToWinterTime.AddDays(-1), 24);
			yield return new TestCaseData(utcDateOfTransitionToWinterTime, 25);
			yield return new TestCaseData(utcDateOfTransitionToWinterTime.AddDays(+1), 24);
		}

		#endregion
	}
}
