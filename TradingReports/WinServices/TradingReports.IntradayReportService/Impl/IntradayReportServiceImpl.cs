using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using TradingReports.Core.Helpers;
using TradingReports.Core.Interfaces;
using TradingReports.Tools.CSV;
using TradingReports.Tools.Reporting;

namespace TradingReports.IntradayReportService.Impl
{
	/// <summary>
	/// Core of Intraday Report Service, which contains all functionality related to reporting.
	/// </summary>
	public class IntradayReportServiceImpl
	{
		private readonly IReportRepository _reportRepository;
		private readonly ICsvService _csvService;
		private readonly ILog _logger = LogManager.GetLogger(typeof(IntradayReportServiceImpl));


		public IntradayReportServiceImpl(IReportRepository reportRepository, ICsvService csvService)
		{
			if (reportRepository == null)
				throw new ArgumentNullException(nameof(reportRepository));
			if (csvService == null)
				throw new ArgumentNullException(nameof(csvService));

			_reportRepository = reportRepository;
			_csvService = csvService;
		}


		/// <summary>
		/// Generates Intraday Report for current date and save report to the file based on provided settings.
		/// </summary>
		/// <param name="reportSettings"></param>
		/// <param name="csvSettings"></param>
		/// <returns></returns>
		public async Task<string> GenerateIntradayReportAsync(ReportSettings reportSettings,
			CsvSettings csvSettings)
		{
			string reportFilePath = null;

			try
			{
				_logger.Info("Intraday Report generation was started.");

				DateTime utcReportDate = DateTime.UtcNow;

				reportFilePath = GenerateReportFilePath(utcReportDate, reportSettings);
				using (var fileStream = File.Create(reportFilePath))
				{
					await this.GenerateIntradayReportAsync(utcReportDate, csvSettings, fileStream);
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

		/// <summary>
		/// Generates Intraday Report for specified date and save report to the file based on provided settings.
		/// </summary>
		/// <param name="utcReportDate"></param>
		/// <param name="csvSettings"></param>
		/// <param name="stream"></param>
		/// <returns></returns>
		public async Task GenerateIntradayReportAsync(DateTime utcReportDate,
			CsvSettings csvSettings, Stream stream)
		{
			var dayTradingData = await _reportRepository.GetDayTradingDataForUkAsync(utcReportDate);
			var streamWriter = new StreamWriter(stream, Encoding.UTF8);

			await _csvService.WriteAsCsv(dayTradingData,
				null, // TODO
				DateHelper.FromUtcToGmt,
				csvSettings,
				streamWriter);
		}


		/// <summary>
		/// Generates unique file path by specified report settings.
		/// <para>File time stamp is generated for UK time zone</para>
		/// </summary>
		/// <param name="utcReportDate"></param>
		/// <param name="reportSettings"></param>
		/// <returns></returns>
		private static string GenerateReportFilePath(DateTime utcReportDate, ReportSettings reportSettings)
		{
			DateTime localReportDate = utcReportDate.FromUtcToGmt(); // convert to UK time
			string fileName = string.Format(reportSettings.FileNameTemplate,
				localReportDate.ToString(reportSettings.FileTimestampFormat));

			// check file name for unique
			int fileVersion = 1;
			string filePath = Path.Combine(reportSettings.FilesDirectory, fileName);
			while (File.Exists(filePath))
			{
				fileVersion++;
				var fileNamePostfix = $"({fileVersion}).";

				filePath = Path.Combine(reportSettings.FilesDirectory, $"{fileName.Replace(".", fileNamePostfix)}");
			}

			return filePath;
		}
	}
}
