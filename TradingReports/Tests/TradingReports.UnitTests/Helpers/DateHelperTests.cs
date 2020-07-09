using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using TradingReports.Core.Helpers;

namespace TradingReports.UnitTests.Helpers
{
	[TestFixture]
	public class DateHelperTests
	{
		[TestCaseSource(nameof(FromTo_Source))]
		public void FromUtcToGmt(DateTime utcTime, DateTime expectedGmtTime)
		{
			// arrange
			
			// act
			var convertedGmtWinterTime = DateHelper.FromUtcToGmt(utcTime);
			
			// assert
			convertedGmtWinterTime.Should().Be(expectedGmtTime);
		}

		[TestCaseSource(nameof(FromTo_Source))]
		public void FromGmtToUtc(DateTime expectedUtcTime, DateTime gmtTime)
		{
			// arrange

			// act
			var convertedUtcWinterTime = DateHelper.FromGmtToUtc(gmtTime);

			// assert
			convertedUtcWinterTime.Should().Be(expectedUtcTime);
		}

		private static IEnumerable<TestCaseData> FromTo_Source()
		{
			var _utcWinterTime = new DateTime(2020, 1, 20, 12, 0, 0);
			var _utcSummerTime = new DateTime(2020, 7, 21, 12, 0, 0);

			var _gmtWinterTime = new DateTime(2020, 1, 20, 12, 0, 0); // UTC+0
			var _gmtSummerTime = new DateTime(2020, 7, 21, 13, 0, 0); // UTC+1


			var _utcDateOfTransitionToSummerTimeBefore = new DateTime(2020, 3, 29, 0, 59, 0);
			var _utcDateOfTransitionToSummerTimeAfter = new DateTime(2020, 3, 29, 1, 01, 0);
			
			var _gmtDateOfTransitionToSummerTimeBefore = new DateTime(2020, 3, 29, 0, 59, 0); // UTC+0
			var _gmtDateOfTransitionToSummerTimeAfter = new DateTime(2020, 3, 29, 2, 01, 0);  // UTC+1


			var _utcDateOfTransitionToWinterTimeBefore = new DateTime(2019, 10, 26, 23, 59, 0);
			var _utcDateOfTransitionToWinterTimeAfter = new DateTime(2019, 10, 27, 1, 01, 0);

			var _gmtDateOfTransitionToWinterTimeBefore = new DateTime(2019, 10, 27, 0, 59, 0); // UTC+1
			var _gmtDateOfTransitionToWinterTimeAfter = new DateTime(2019, 10, 27, 1, 01, 0);  // UTC+0

			// TestCaseData: { utcTime, gmtTime }
			yield return new TestCaseData(_utcWinterTime, _gmtWinterTime);
			yield return new TestCaseData(_utcSummerTime, _gmtSummerTime);
			
			yield return new TestCaseData(_utcDateOfTransitionToSummerTimeBefore, _gmtDateOfTransitionToSummerTimeBefore);
			yield return new TestCaseData(_utcDateOfTransitionToSummerTimeAfter, _gmtDateOfTransitionToSummerTimeAfter);

			yield return new TestCaseData(_utcDateOfTransitionToWinterTimeBefore, _gmtDateOfTransitionToWinterTimeBefore);
			yield return new TestCaseData(_utcDateOfTransitionToWinterTimeAfter, _gmtDateOfTransitionToWinterTimeAfter);
		}


		[Test]
		public void GetLocalStartTimeOfTradingDay()
		{
			// arrange
			var tradingDate = new DateTime(2020, 7, 9, 13, 22, 33);
			var expectedStartTimeOfTradingDay = new DateTime(2020, 7, 8, 23, 0, 0);

			// act
			var testedStartTimeOfTradingDay = DateHelper.GetLocalStartTimeOfTradingDay(tradingDate);

			// assert
			testedStartTimeOfTradingDay.Should().Be(expectedStartTimeOfTradingDay);
		}
	}
}
