using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradingReports.Core.Helpers;
using TradingReports.Core.Interfaces;
using TradingReports.Tools.CSV;
using TradingReports.Tools.Reporting;

namespace TradingReports.IntradayReportService.Managers
{
	public class ReportManager
	{
		private readonly IReportRepository _reportRepository;


		public ReportManager(IReportRepository reportRepository)
		{
			if(reportRepository == null)
				throw new ArgumentNullException(nameof(reportRepository));

			_reportRepository = reportRepository;
		}


		public async Task<string> GenerateIntradateReportAsync()
		{
			try
			{
				DateTime utcReportDate = DateTime.UtcNow;

				var dayTradingData = await _reportRepository.GetDayTradingDataForUkAsync(utcReportDate);

				string reportFilePath = ReportHelper.GenerateReportFilePath(utcReportDate);
				using (var fileStream = File.Create(reportFilePath))
				{
					var streamWriter = new StreamWriter(fileStream, Encoding.UTF8);
					CsvHelper.WriteToCsv(dayTradingData, 
						null, // TODO
						DateHelper.FromUtcToGmt,
						streamWriter);
				}

				return reportFilePath;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
	}
}
