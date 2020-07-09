using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TradingReports.Core.Helpers;

namespace TradingReports.UnitTests.Helpers
{
	[TestFixture]
	public class DateHelperTests
	{
		private readonly DateTime _utcWinterTime = new DateTime(2020, 1, 20, 12, 0, 0);
		private readonly DateTime _utcSummerTime = new DateTime(2020, 7, 21, 12, 0, 0);

		private readonly DateTime _gmtWinterTime = new DateTime(2020, 1, 20, 12, 0, 0); // UTC+0
		private readonly DateTime _gmtSummerTime = new DateTime(2020, 7, 21, 13, 0, 0); // UTC+1


		[Test]
		public void FromUtcToGmt()
		{
			// arrange
			
			// act
			var convertedGmtWinterTime = DateHelper.FromUtcToGmt(_utcWinterTime);
			var convertedGmtSummerTime = DateHelper.FromUtcToGmt(_utcSummerTime);

			// assert
			convertedGmtWinterTime.Should().Be(_gmtWinterTime);
			convertedGmtSummerTime.Should().Be(_gmtSummerTime);
		}

		[Test]
		public void FromGmtToUtc()
		{
			// arrange

			// act
			var convertedUtcWinterTime = DateHelper.FromGmtToUtc(_gmtWinterTime);
			var convertedUtcSummerTime = DateHelper.FromGmtToUtc(_gmtSummerTime);

			// assert
			convertedUtcWinterTime.Should().Be(_utcWinterTime);
			convertedUtcSummerTime.Should().Be(_utcSummerTime);
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
