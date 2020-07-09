using System;
using System.Timers;
using TradingReports.Core.DAL;
using TradingReports.Core.Helpers;
using TradingReports.Core.Interfaces;
using TradingReports.Core.Repositories;
using TradingReports.IntradayReportService.Managers;
using TradingReports.Tools.Reporting;
using Unity;

namespace TradingReports.IntradayReportConsole
{
	class Program
	{
		private static IUnityContainer _container;

		static void Main(string[] args)
		{
			PrintCurrentSettings();

			_container = BuildContainer();
			
			// Set up a timer
			Timer timer = new Timer();
			timer.Interval = ReportHelper.Settings.ReportingIntervalInMinutes * 60000; // min * 60 seconds
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
			var reportManager = _container.Resolve<ReportManager>();
			await reportManager.GenerateIntradayReportAsync();
			
			Console.WriteLine("---");
		}
		

		private static IUnityContainer BuildContainer()
		{
			IUnityContainer container = new UnityContainer();

			container.RegisterType<ITradingDataAdapter, TradingDataAdapter>();
			container.RegisterType<IReportRepository, ReportRepository>();

			return container;
		}

		private static void PrintCurrentSettings()
		{
			Console.WriteLine("Current UK date: {0}", DateTime.UtcNow.FromUtcToGmt().ToString("dd MMMM yyyy HH:mm"));
			Console.WriteLine("");

			Console.WriteLine("Report Settings:");
			Console.WriteLine("\t - FileNameTemplate: {0}", ReportHelper.Settings.FileNameTemplate);
			Console.WriteLine("\t - FileTimestampFormat: {0}", ReportHelper.Settings.FileTimestampFormat);
			Console.WriteLine("\t - FilesDirectory: {0}", ReportHelper.Settings.FilesDirectory);
			Console.WriteLine("\t - ReportingIntervalInMinutes: {0}", ReportHelper.Settings.ReportingIntervalInMinutes);
			Console.WriteLine("");
		}
	}
}
