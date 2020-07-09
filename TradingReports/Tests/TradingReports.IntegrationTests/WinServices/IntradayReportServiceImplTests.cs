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
using TradingReports.Core.DAL;
using TradingReports.Core.Interfaces;
using TradingReports.Core.Repositories;
using TradingReports.IntradayReportService.Impl;
using TradingReports.Tools.CSV;

namespace TradingReports.IntegrationTests.WinServices
{
	public class IntradayReportServiceImplTests
	{
		// date of transition to summer time in UK 
		private static DateTime _utcDateOfTransitionToSummerTime = new DateTime(2020, 3, 29, 1, 0, 0);
		// date of transition to winter time in UK
		private static DateTime _utcDateOfTransitionToWinterTime = new DateTime(2019, 10, 27, 2, 0, 0);

		private readonly CsvSettings _csvSettings = CsvSettings.ReadFromConfig();


		// Test whole system behavior.

		// Tested integration schemas are the following:
		//   1) IntradayReportServiceImplTests -> CsvService, ReportRepository -> mock of TradingDataAdapter
		//   2) IntradayReportServiceImplTests -> CsvService, ReportRepository -> TradingDataAdapter -> TradingPlatform.TradingService

		// Tests are written for two special cases:
		//  1) Daylight Savings Time change (1am -> 2am, UTC+0 -> UTC+1)
		//  2) Transition to winter time (2am -> 1am, UTC+1 -> UTC+0)


		#region GenerateIntradayReportAsync_TransitionToSummerTime

		[TestCaseSource(nameof(GenerateIntradayReportAsync_TransitionToSummerTime_Source))]
		public async Task GenerateIntradayReportAsync_TransitionToSummerTime(ITradingDataAdapter tradingDataAdapter)
		{
			// arrange
			var reportRepository = new ReportRepository(tradingDataAdapter);
			var csvService = new CsvService();
			var reportService = new IntradayReportServiceImpl(reportRepository, csvService);

			// act
			string csv = null;
			using (MemoryStream ms = new MemoryStream())
			{
				await reportService.GenerateIntradayReportAsync(_utcDateOfTransitionToSummerTime, _csvSettings, ms);

				ms.Seek(0, SeekOrigin.Begin);
				csv = new StreamReader(ms, Encoding.UTF8).ReadToEnd();
			}

			// assert:

			csv.Should().NotBeNullOrEmpty();

			var csvRows = csv.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

			// Spec.: "CSV should only contain local time, use the GMT time zone periods as the number in the CSV (23 in this case)"
			csvRows.Length.Should().Be(23 + 1); // data rows + header row

			// Check that report doesn't contain 1am period
			DateTime excludedPeriodDate = new DateTime(2020, 3, 29, 1, 0, 0);
			Assert.IsTrue(!csvRows.Any(r => r.Contains(excludedPeriodDate.ToString(_csvSettings.TimeFormat))));
		}

		private static IEnumerable<TestCaseData> GenerateIntradayReportAsync_TransitionToSummerTime_Source()
		{
			// Test whole system behavior, but with mock data source.
			var tradingDataAdapterMock = GetTradingDataAdapterMockForTransToSummerTime();
			yield return new TestCaseData(tradingDataAdapterMock.Object);

			// Test whole system behavior, with real data source.
			yield return new TestCaseData(new TradingDataAdapter());
			
		}
		
		private static Mock<ITradingDataAdapter> GetTradingDataAdapterMockForTransToSummerTime()
		{
			var data = new
			{
				Trades = new List<Trade>(),
				TradePeriodCnt = 23
			};

			data.Trades.Add(Trade.Create(_utcDateOfTransitionToSummerTime, data.TradePeriodCnt));
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

			return tradingDataAdapter;
		}

		#endregion

		#region GenerateIntradayReportAsync_TransitionToWinterTime

		[TestCaseSource(nameof(GenerateIntradayReportAsync_TransitionToWinterTime_Source))]
		public async Task GenerateIntradayReportAsync_TransitionToWinterTime(ITradingDataAdapter tradingDataAdapter,
			double? volumeValueFor1amUtc, double? volumeValueFor2amUtc)
		{
			// arrange
			var reportRepository = new ReportRepository(tradingDataAdapter);
			var csvService = new CsvService();
			var reportService = new IntradayReportServiceImpl(reportRepository, csvService);
			
			// act
			string csv = null;
			using (MemoryStream ms = new MemoryStream())
			{
				await reportService.GenerateIntradayReportAsync(_utcDateOfTransitionToWinterTime, _csvSettings, ms);

				ms.Seek(0, SeekOrigin.Begin);
				csv = new StreamReader(ms, Encoding.UTF8).ReadToEnd();
			}

			// assert:

			csv.Should().NotBeNullOrEmpty();

			var csvRows = csv.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

			// Spec.: "CSV should only contain local time, use the GMT time zone periods as the number in the CSV (25 in this case)"
			csvRows.Length.Should().Be(25 + 1); // data rows + header row

			// Check that report contains two local 1am periods
			DateTime repeatedPeriodDate = new DateTime(2020, 3, 29, 1, 0, 0);
			var rowsForRepeatedPeriod = csvRows
				.Where(r => r.Contains(repeatedPeriodDate.ToString(_csvSettings.TimeFormat)))
				.ToArray();
			rowsForRepeatedPeriod.Length.Should().Be(2);

			// for mock data we can check additional system behavior
			if (volumeValueFor1amUtc != null && volumeValueFor2amUtc != null)
			{
				// Check that repeated rows contains valid Volume values (for 1am and for 2am UTC period)
				Assert.IsTrue(rowsForRepeatedPeriod.Any(r =>
					r.Contains(volumeValueFor1amUtc.Value.ToString(_csvSettings.NumberFormat))));
				Assert.IsTrue(rowsForRepeatedPeriod.Any(r =>
					r.Contains(volumeValueFor2amUtc.Value.ToString(_csvSettings.NumberFormat))));
			}
		}

		private static IEnumerable<TestCaseData> GenerateIntradayReportAsync_TransitionToWinterTime_Source()
		{
			// Test whole system behavior, but with mock data source.
			var tradingDataAdapterMock = GetTradingDataAdapterMockForTransToWinterTime();
			yield return new TestCaseData(tradingDataAdapterMock.Object, 11.111, 22.222);

			// Test whole system behavior, with real data source.
			yield return new TestCaseData(new TradingDataAdapter(), null, null);

		}

		private static Mock<ITradingDataAdapter> GetTradingDataAdapterMockForTransToWinterTime()
		{
			var data = new
			{
				Trades = new List<Trade>(),
				TradePeriodCnt = 25
			};
			
			data.Trades.Add(Trade.Create(_utcDateOfTransitionToWinterTime, data.TradePeriodCnt));
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

			return tradingDataAdapter;
		}

		#endregion
	}
}