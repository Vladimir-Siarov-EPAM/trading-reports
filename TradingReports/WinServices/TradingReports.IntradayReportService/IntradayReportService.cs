using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Common.Logging;
using TradingReports.Core.DAL;
using TradingReports.Core.Interfaces;
using TradingReports.Core.Repositories;
using TradingReports.IntradayReportService.Managers;
using TradingReports.Tools.Reporting;
using Unity;

namespace TradingReports.IntradayReportService
{
	public partial class IntradayReportService : ServiceBase
	{
		private readonly ILog _logger = LogManager.GetLogger(typeof(IntradayReportService));
		private readonly IUnityContainer _container;


		public IntradayReportService()
		{
			InitializeComponent();

			_container = BuildContainer();
		}

		protected override void OnStart(string[] args)
		{
			_logger.Info("Starting...");

			// Set up a timer that triggers every minute.
			Timer timer = new Timer();
			timer.Interval = ReportHelper.Settings.ReportingIntervalInMinutes * 60000; // min * 60 seconds
			timer.Elapsed += TimerOnElapsed;
			timer.Start();

			// Start generate report immediately
			TimerOnElapsed(null, null);
		}

		protected override void OnStop()
		{
			_logger.Info("Stopping...");
		}

		private async void TimerOnElapsed(object sender, ElapsedEventArgs e)
		{
			var reportManager = _container.Resolve<ReportManager>();
			await reportManager.GenerateIntradayReportAsync();
		}

		private IUnityContainer BuildContainer()
		{
			IUnityContainer container = new UnityContainer();

			container.RegisterType<ITradingDataAdapter, TradingDataAdapter>();
			container.RegisterType<IReportRepository, ReportRepository>();

			return container;
		}
	}
}
