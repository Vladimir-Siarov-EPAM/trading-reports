using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TradingPlatform;
using TradingReports.Core.Helpers;
using TradingReports.Core.Interfaces;
using TradingReports.Core.Repositories;

namespace TradingReports.UnitTests.Repositories
{
	[TestFixture]
	public class ReportRepositoryTests
	{
		#region GetDayTradingDataForUkAsync

		[TestCaseSource(nameof(GetDayTradingDataForUkAsync_Source))]
		public async Task GetDayTradingDataForUkAsync(DateTime utcReportDate, int expectedPeriodCnt)
		{
			#region arrange

			var data = new
			{
				Trades = new List<Trade>()
			};
			data.Trades.Add(Trade.Create(utcReportDate.FromUtcToGmt(), expectedPeriodCnt));
			data.Trades.Add(Trade.Create(utcReportDate.FromUtcToGmt(), expectedPeriodCnt));
			foreach (Trade trade in data.Trades)
			{
				for (int i = 0; i < trade.Periods.Length; i++)
				{
					trade.Periods[i].Volume = (i + 1) * 11.111;
				}
			}

			var tradingDataAdapter = new Mock<ITradingDataAdapter>();
			{
				tradingDataAdapter
					.Setup(x => x.GetTradesSafetyAsync(It.IsAny<DateTime>()))
					.Returns(Task.FromResult((IEnumerable<Trade>)data.Trades));
			}

			#endregion

			var reportRepository = new ReportRepository(tradingDataAdapter.Object);

			// act
			var testedTrades = (await reportRepository.GetDayTradingDataForUkAsync(utcReportDate)).ToArray();

			// assert
			testedTrades.Should().NotBeNull();
			testedTrades.Length.Should().Be(expectedPeriodCnt);

			int uniquePeriodUtcDatesCnt = testedTrades.Select(t => t.PeriodUtcDate).Distinct().Count();
			uniquePeriodUtcDatesCnt.Should().Be(expectedPeriodCnt);

			double[] orderedVolumes = testedTrades.OrderBy(t => t.PeriodUtcDate).Select(t => t.Volume).ToArray();
			for (int i = 0; i < expectedPeriodCnt; i++)
			{
				orderedVolumes[i].Should().Be(data.Trades.Sum(t => t.Periods[i].Volume));
			}
		}

		private static IEnumerable<TestCaseData> GetDayTradingDataForUkAsync_Source()
		{
			var utcDateOfTransitionToSummerTime = new DateTime(2020, 3, 29, 1, 0, 0);
			var utcDateOfTransitionToWinterTime = new DateTime(2019, 10, 27, 2, 0, 0);

			// TestCaseData: { utcReportDate, expectedTradingPeriodCnt }
			yield return new TestCaseData(utcDateOfTransitionToSummerTime.AddDays(-1), 24);
			yield return new TestCaseData(utcDateOfTransitionToSummerTime, 23);

			yield return new TestCaseData(utcDateOfTransitionToWinterTime, 25);
			yield return new TestCaseData(utcDateOfTransitionToWinterTime.AddDays(+1), 24);
		}

		#endregion
	}
}
