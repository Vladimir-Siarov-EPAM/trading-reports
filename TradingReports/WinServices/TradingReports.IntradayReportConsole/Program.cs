using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
		static async Task Main(string[] args)
		{
			Console.WriteLine("Current UK date: {0}", DateTime.UtcNow.FromUtcToGmt().ToString("dd MMMM yyyy HH:mm"));
			Console.WriteLine("");

			Console.WriteLine("Report Settings:");
			Console.WriteLine("\t - FileNameTemplate: {0}", ReportHelper.Settings.FileNameTemplate);
			Console.WriteLine("\t - FileTimestampFormat: {0}", ReportHelper.Settings.FileTimestampFormat);
			Console.WriteLine("\t - FilesDirectory: {0}", ReportHelper.Settings.FilesDirectory);
			Console.WriteLine("\t - ReportingIntervalInMinutes: {0}", ReportHelper.Settings.ReportingIntervalInMinutes);
			Console.WriteLine("");

			var container = BuildContainer();
			var reportManager = container.Resolve<ReportManager>();

			Console.WriteLine("Start to generate report...");
			string reportPath = await reportManager.GenerateIntradayReportAsync();
			//Console.WriteLine("Report was generated: {0}", reportPath);

			Console.ReadKey();
		}


		private static IUnityContainer BuildContainer()
		{
			IUnityContainer container = new UnityContainer();

			container.RegisterType<ITradingDataAdapter, TradingDataAdapter>();
			container.RegisterType<IReportRepository, ReportRepository>();

			return container;
		}
	}
}
