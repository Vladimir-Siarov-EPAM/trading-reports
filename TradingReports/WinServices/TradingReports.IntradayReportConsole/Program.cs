using System;
using System.Timers;
using TradingReports.Core.DAL;
using TradingReports.Core.Helpers;
using TradingReports.Core.Interfaces;
using TradingReports.Core.Repositories;
using TradingReports.IntradayReportService.Impl;
using TradingReports.Tools.CSV;
using TradingReports.Tools.Reporting;
using Unity;

namespace TradingReports.IntradayReportConsole
{
	class Program
	{
		private static IUnityContainer _container;
		private static readonly ReportSettings _reportSettings = ReportSettings.ReadFromConfig();
		private static readonly CsvSettings _csvSettings = CsvSettings.ReadFromConfig();


		static void Main(string[] args)
		{
			PrintCurrentSettings();

			_container = BuildContainer();
			
			// Set up a timer
			Timer timer = new Timer();
			timer.Interval = _reportSettings.ReportingIntervalInMinutes * 60000; // min * 60 seconds
			timer.Elapsed += TimerOnElapsed;
			timer.Start();

			// Start generate report immediately
			TimerOnElapsed(null, null);

			// Console.WriteLine("Start to generate report...");
			// string reportPath = await reportManager.GenerateIntradayReportAsync();
			//Console.WriteLine("Report was generated: {0}", reportPath);

			Console.ReadKey();
		}

		private static async void TimerOnElapsed(object sender, ElapsedEventArgs e)
		{
			var reportService = _container.Resolve<IntradayReportServiceImpl>();
			await reportService.GenerateIntradayReportAsync(_reportSettings, _csvSettings);
			
			Console.WriteLine("---");
		}
		

		private static IUnityContainer BuildContainer()
		{
			IUnityContainer container = new UnityContainer();

			container.RegisterType<ITradingDataAdapter, TradingDataAdapter>();
			container.RegisterType<IReportRepository, ReportRepository>();
			container.RegisterType<ICsvService, CsvService>();

			return container;
		}

		private static void PrintCurrentSettings()
		{
			Console.WriteLine("Current UK date: {0}", DateTime.UtcNow.FromUtcToGmt().ToString("dd MMMM yyyy HH:mm"));
			Console.WriteLine("");

			Console.WriteLine("Report Settings:");
			Console.WriteLine("\t - FileNameTemplate: {0}", _reportSettings.FileNameTemplate);
			Console.WriteLine("\t - FileTimestampFormat: {0}", _reportSettings.FileTimestampFormat);
			Console.WriteLine("\t - FilesDirectory: {0}", _reportSettings.FilesDirectory);
			Console.WriteLine("\t - ReportingIntervalInMinutes: {0}", _reportSettings.ReportingIntervalInMinutes);
			Console.WriteLine("");
		}
	}
}
