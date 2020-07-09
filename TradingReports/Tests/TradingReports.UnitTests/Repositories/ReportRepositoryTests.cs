using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TradingReports.Core.DAL;
using TradingReports.Core.Repositories;

namespace TradingReports.UnitTests.Repositories
{
	[TestFixture]
	public class ReportRepositoryTests
	{
		[Test]
		public async Task GetDayTradingDataForUkAsync()
		{
			var tradingAdapter = new TradingDataAdapter();
			var reportRepository = new ReportRepository(tradingAdapter);

			var rez = await reportRepository.GetDayTradingDataForUkAsync(DateTime.UtcNow);
		}
	}
}
