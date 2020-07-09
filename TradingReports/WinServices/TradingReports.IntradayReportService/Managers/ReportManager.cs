using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using TradingReports.Core.Helpers;
using TradingReports.Core.Interfaces;
using TradingReports.Tools.CSV;
using TradingReports.Tools.Reporting;

namespace TradingReports.IntradayReportService.Managers
{
	public class ReportManager
	{
		private readonly IReportRepository _reportRepository;
		private readonly ILog _logger = LogManager.GetLogger(typeof(ReportManager));


		public ReportManager(IReportRepository reportRepository)
		{
			if(reportRepository == null)
				throw new ArgumentNullException(nameof(reportRepository));

			_reportRepository = reportRepository;
		}


		public async Task<string> GenerateIntradayReportAsync()
		{
			string reportFilePath = null;

			try
			{
				_logger.Info("Intraday Report generation was started.");

				DateTime utcReportDate = DateTime.UtcNow;
				
				reportFilePath = ReportHelper.GenerateReportFilePath(utcReportDate);
				using (var fileStream = File.Create(reportFilePath))
				{
					await this.GenerateIntradayReportAsync(utcReportDate, fileStream);
				}

				_logger.InfoFormat("Intraday Report was generated successfully. File: {0}", reportFilePath);

				return reportFilePath;
			}
			catch (Exception ex)
			{
				_logger.ErrorFormat("Intraday Report generation failed. File: {0}", ex, reportFilePath);
				throw;
			}
		}

		// public async Task<string> GenerateIntradayReportAsync()
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
		internal async Task GenerateIntradayReportAsync(DateTime utcReportDate, Stream stream)
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
