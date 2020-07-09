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
				
				string reportFilePath = ReportHelper.GenerateReportFilePath(utcReportDate);
				using (var fileStream = File.Create(reportFilePath))
				{
					await this.GenerateIntradateReportAsync(utcReportDate, fileStream);
				}

				return reportFilePath;
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		// public async Task<string> GenerateIntradateReportAsync()
		// {
		// 	try
		// 	{
		// 		DateTime utcReportDate = DateTime.UtcNow;
		//
		// 		var dayTradingData = await _reportRepository.GetDayTradingDataForUkAsync(utcReportDate);
		//
		// 		string reportFilePath = ReportHelper.GenerateReportFilePath(utcReportDate);
		// 		using (var fileStream = File.Create(reportFilePath))
		// 		{
		// 			var streamWriter = new StreamWriter(fileStream, Encoding.UTF8);
		// 			await CsvHelper.WriteAsCsv(dayTradingData,
		// 				null, // TODO
		// 				DateHelper.FromUtcToGmt,
		// 				streamWriter);
		// 		}
		//
		// 		return reportFilePath;
		// 	}
		// 	catch (Exception ex)
		// 	{
		// 		throw;
		// 	}
		// }

		// internal - for tests support
		internal async Task GenerateIntradateReportAsync(DateTime utcReportDate, Stream stream)
		{
			var dayTradingData = await _reportRepository.GetDayTradingDataForUkAsync(utcReportDate);
			var streamWriter = new StreamWriter(stream, Encoding.UTF8);

			await CsvHelper.WriteAsCsv(dayTradingData,
				null, // TODO
				DateHelper.FromUtcToGmt,
				streamWriter);
		}
	}
}
