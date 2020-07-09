using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TradingPlatform;
using TradingReports.Core.Interfaces;
using TradingReports.Core.Repositories;
using TradingReports.IntradayReportService.Managers;
using TradingReports.Tools.CSV;

namespace TradingReports.UnitTests.Managers
{
	[TestFixture]
	public class ReportManagerTests
	{
		// It looks like more as integration tests, because we test whole system behavior, but with mock data source.

		// Tests for special cases:
		//  1) Daylight Savings Time change (1am -> 2am, UTC+0 -> UTC+1)
		//  2) Transition to winter time (2am -> 1am, UTC+1 -> UTC+0)

		[Test]
		public async Task GenerateIntradateReportAsync_TransitionToSummerTime()
		{
			#region arrange

			var data = new
			{
				UtcReportDate = new DateTime(2020, 3, 29, 1, 0, 0), // date of transition to summer time in UK
				Trades = new List<Trade>(),
				TradePeriodCnt = 23
			};
			data.Trades.Add(Trade.Create(data.UtcReportDate, data.TradePeriodCnt));
			foreach (Trade trade in data.Trades)
			{
				trade.Periods[0].Volume = 23 * 11.111;
				for (int i = 1; i < trade.Periods.Length; i++)
				{
					trade.Periods[i].Volume = (i - 1) * 11.111;
				}
			}

			var tradingDataAdapter = new Mock<ITradingDataAdapter>();
			{
				tradingDataAdapter
					.Setup(x => x.GetTradesSafetyAsync(It.IsAny<DateTime>()))
					.Returns(Task.FromResult((IEnumerable<Trade>)data.Trades));
			}

			var reportRepository = new ReportRepository(tradingDataAdapter.Object);
			var reportManager = new ReportManager(reportRepository);

			#endregion

			// act
			string csv = null;
			using (MemoryStream ms = new MemoryStream())
			{
				await reportManager.GenerateIntradateReportAsync(data.UtcReportDate, ms);
				
				ms.Seek(0, SeekOrigin.Begin);
				StreamReader sr = new StreamReader(ms, Encoding.UTF8);
				csv = sr.ReadToEnd();
			}

			// assert:

			csv.Should().NotBeNullOrEmpty();

			var csvRows = csv.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

			// Spec.: "CSV should only contain local time, use the GMT time zone periods as the number in the CSV (23 in this case)"
			csvRows.Length.Should().Be(data.TradePeriodCnt + 1); // data rows + header row

			// Check that report doesn't contain 1am period
			DateTime excludedPeriodDate = new DateTime(2020, 3, 29, 1, 0, 0);
			Assert.IsTrue(!csvRows.Any(r => r.Contains(excludedPeriodDate.ToString(CsvHelper.DefaultCsvSettings.TimeFormat))));
		}

		[Test]
		public async Task GenerateIntradateReportAsync_TransitionToWinterTime()
		{
			#region arrange

			var data = new
			{
				UtcReportDate = new DateTime(2019, 10, 27, 2, 0, 0), // date of transition to winter time in UK
				Trades = new List<Trade>(),
				TradePeriodCnt = 25
			};
			data.Trades.Add(Trade.Create(data.UtcReportDate, data.TradePeriodCnt));
			foreach (Trade trade in data.Trades)
			{
				trade.Periods[0].Volume = 23 * 11.111;
				for (int i = 1; i < trade.Periods.Length; i++)
				{
					trade.Periods[i].Volume = (i-1) * 11.111;
				}
			}

			var tradingDataAdapter = new Mock<ITradingDataAdapter>();
			{
				tradingDataAdapter
					.Setup(x => x.GetTradesSafetyAsync(It.IsAny<DateTime>()))
					.Returns(Task.FromResult((IEnumerable<Trade>)data.Trades));
			}

			var reportRepository = new ReportRepository(tradingDataAdapter.Object);
			var reportManager = new ReportManager(reportRepository);

			#endregion

			// act
			string csv = null;
			using (MemoryStream ms = new MemoryStream())
			{
				await reportManager.GenerateIntradateReportAsync(data.UtcReportDate, ms);

				ms.Seek(0, SeekOrigin.Begin);
				StreamReader sr = new StreamReader(ms, Encoding.UTF8);
				csv = sr.ReadToEnd();
			}

			// assert:

			csv.Should().NotBeNullOrEmpty();

			var csvRows = csv.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

			// Spec.: "CSV should only contain local time, use the GMT time zone periods as the number in the CSV (25 in this case)"
			csvRows.Length.Should().Be(data.TradePeriodCnt + 1); // data rows + header row

			// Check that report contains two local 1am periods
			DateTime repeatedPeriodDate = new DateTime(2020, 3, 29, 1, 0, 0);
			var rowsForRepeatedPeriod = csvRows
				.Where(r => r.Contains(repeatedPeriodDate.ToString(CsvHelper.DefaultCsvSettings.TimeFormat)))
				.ToArray();
			rowsForRepeatedPeriod.Length.Should().Be(2);

			// Check that repeated rows contains valid Volume values (for 1am and for 2am UTC period)
			Assert.IsTrue(rowsForRepeatedPeriod.Any(r => r.Contains(11.111.ToString(CsvHelper.DefaultCsvSettings.NumberFormat))));
			Assert.IsTrue(rowsForRepeatedPeriod.Any(r => r.Contains(22.222.ToString(CsvHelper.DefaultCsvSettings.NumberFormat))));
		}
	}
}
