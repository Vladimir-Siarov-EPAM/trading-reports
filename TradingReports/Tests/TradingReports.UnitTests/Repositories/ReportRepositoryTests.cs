using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TradingPlatform;
using TradingReports.Core.DAL;
using TradingReports.Core.Interfaces;
using TradingReports.Core.Repositories;

namespace TradingReports.UnitTests.Repositories
{
	[TestFixture]
	public class ReportRepositoryTests
	{
		[Test]
		public async Task GetDayTradingDataForUkAsync()
		{
			#region arrange

			var data = new
			{
				GmtDate = new DateTime(2020, 3, 29, 13, 0, 0),
				Trades = new List<Trade>(),
				TradePeriodCnt = 23 
			};
			data.Trades.Add(Trade.Create(data.GmtDate, data.TradePeriodCnt));
			data.Trades.Add(Trade.Create(data.GmtDate, data.TradePeriodCnt));
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
			var testedTrades = (await reportRepository.GetDayTradingDataForUkAsync(data.GmtDate)).ToArray();

			// assert
			testedTrades.Should().NotBeNull();
			testedTrades.Length.Should().Be(data.TradePeriodCnt);

			int uniquePeriodUtcDatesCnt = testedTrades.Select(t => t.PeriodUtcDate).Distinct().Count();
			uniquePeriodUtcDatesCnt.Should().Be(data.TradePeriodCnt);

			double[] orderedVolumes = testedTrades.OrderBy(t => t.PeriodUtcDate).Select(t => t.Volume).ToArray();
			for (int i = 0; i < data.TradePeriodCnt; i++)
			{
				orderedVolumes[i].Should().Be(data.Trades.Sum(t => t.Periods[i].Volume));
			}
		}
	}
}
