using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradingReports.Core.BE;
using TradingReports.Core.Helpers;
using TradingReports.Core.Interfaces;

namespace TradingReports.Core.Repositories
{
	public class ReportRepository : BaseRepository, IReportRepository
	{
		private readonly ITradingDataAdapter _tradingDataAdapter;


		public ReportRepository(ITradingDataAdapter tradingDataAdapter)
			: base()
		{
			if(tradingDataAdapter == null)
				throw new ArgumentNullException(nameof(tradingDataAdapter));

			_tradingDataAdapter = tradingDataAdapter;
		}


		#region IReportRepository

		/// <inheritdoc/>
		public async Task<IEnumerable<TradingHourlyData>> GetDayTradingDataForUkAsync(DateTime utcDate)
		{
			try
			{
				var localDate = utcDate.FromUtcToGmt(); // UK local date
				var trades = await _tradingDataAdapter.GetTradesSafetyAsync(localDate);

				Dictionary<int, double> aggregatedPeriodsAndVolumes = trades
					.SelectMany(t => t.Periods)
					.GroupBy(p => p.Period)
					.ToDictionary(g => g.Key, g => g.Sum(p => p.Volume));

				var localStartTimeOfTradingDay = localDate.GetLocalStartTimeOfTradingDay();
				var utcStartTimeOfTradingDay = localStartTimeOfTradingDay.FromGmtToUtc();

				var dayTradingData = aggregatedPeriodsAndVolumes
					.Select(periodAndVolumePair => new TradingHourlyData
						{
							PeriodUtcDate = utcStartTimeOfTradingDay.AddHours(periodAndVolumePair.Key - 1), // period numbers start from 1
							Volume = periodAndVolumePair.Value
						})
					.ToArray();

				return dayTradingData;
			}
			catch (Exception ex)
			{
				this.Logger.Error("Error has occurred on \"GetDayTradingDataForUkAsync\" method", ex);
				throw;
			}
		}

		#endregion
	}
}
