using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using TradingReports.Core.BE;
using TradingReports.Core.Helpers;
using TradingReports.Tools.CSV;

namespace TradingReports.UnitTests.Helpers
{
	[TestFixture]
	public class CsvHelperTests
	{
		#region WriteToCsv

		[TestCaseSource(nameof(WriteToCsv_Source))]
		public void WriteToCsv(IEnumerable<TradingHourlyData> dayTradingData,
			string[] headers, Func<DateTime, DateTime> fnToLocalTime, CsvSettings customCsvSettings,
			string[] expectedCsvRows)
		{
			// arrange
			
			// act
			string csv = null;
			using (MemoryStream ms = new MemoryStream())
			{
				StreamWriter sw = new StreamWriter(ms, Encoding.UTF8);
				CsvHelper.WriteToCsv(dayTradingData, headers, fnToLocalTime, sw, customCsvSettings);

				ms.Seek(0, SeekOrigin.Begin);
				StreamReader sr = new StreamReader(ms, Encoding.UTF8);
				csv = sr.ReadToEnd();
			}

			// assert
			csv.Should().NotBeNullOrEmpty();

			var csvRows = csv.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
			csvRows.Length.Should().Be(dayTradingData.Count() + 1);

			if (expectedCsvRows != null)
			{
				for (int i = 0; i < expectedCsvRows.Length; i++)
				{
					csvRows[i].Should().BeEquivalentTo(expectedCsvRows[i]);
				}
			}
		}

		private static IEnumerable<TestCaseData> WriteToCsv_Source()
		{
			string[] headers = new[] {"Local Time Header", "Volume Header"};
			TradingHourlyData[] dayTradingData = new[]
			{
				new TradingHourlyData{ PeriodUtcDate = new DateTime(2020, 7, 8, 22, 0, 0), Volume = 123.456789 },
				new TradingHourlyData{ PeriodUtcDate = new DateTime(2020, 1, 8, 23, 0, 0), Volume = 0.123456789 }
			};
			
			// check default settings and nullable parameters
			yield return new TestCaseData(dayTradingData, null, null, null, null);

			// check different CSV setting combinations
			yield return new TestCaseData(dayTradingData, headers, null, 
				new CsvSettings
				{
					FieldSeparator = ";",
					Qualifier = "",
					TimeFormat = "HH:mm:ss zzz",
					NumberFormat = "000.####"
				}, 
				new[]
				{
					"Local Time Header;Volume Header",
					"22:00:00 +03:00;123.4568",
					"23:00:00 +03:00;000.1235"
				});
			yield return new TestCaseData(dayTradingData, headers, null,
				new CsvSettings
				{
					FieldSeparator = "\t",
					Qualifier = "\"",
					TimeFormat = "yyyy MMM dd",
					NumberFormat = "#.####"
				},
				new[]
				{
					"\"Local Time Header\"\t\"Volume Header\"",
					"\"2020 Jul 08\"\t\"123.4568\"",
					"\"2020 Jan 08\"\t\".1235\""
				});

			// check conversion to local time
			Func<DateTime, DateTime> fnToLocalTime = DateHelper.FromUtcToGmt;
			yield return new TestCaseData(dayTradingData, headers, fnToLocalTime,
				new CsvSettings
				{
					FieldSeparator = ",",
					Qualifier = "",
					TimeFormat = "MM/dd/yyyy HH:mm",
					NumberFormat = "0.####"
				},
				new[]
				{
					"Local Time Header,Volume Header",
					"07/08/2020 23:00,123.4568", // summer time UTC+1
					"01/08/2020 23:00,0.1235"    // winter time UTC+0
				});
		}

		#endregion
	}
}
