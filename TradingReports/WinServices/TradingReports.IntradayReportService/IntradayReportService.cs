using System.ServiceProcess;
using System.Timers;
using Common.Logging;
using TradingReports.Core.DAL;
using TradingReports.Core.Interfaces;
using TradingReports.Core.Repositories;
using TradingReports.IntradayReportService.Impl;
using TradingReports.Tools.CSV;
using TradingReports.Tools.Reporting;
using Unity;

namespace TradingReports.IntradayReportService
{
	public partial class IntradayReportService : ServiceBase
	{
		private readonly ILog _logger = LogManager.GetLogger(typeof(IntradayReportService));
		private readonly IUnityContainer _container;
		private readonly ReportSettings _reportSettings = ReportSettings.ReadFromConfig();
		private readonly CsvSettings _csvSettings = CsvSettings.ReadFromConfig();


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
			timer.Interval = _reportSettings.ReportingIntervalInMinutes * 60000; // min * 60 seconds
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
			var reportService = _container.Resolve<IntradayReportServiceImpl>();
			await reportService.GenerateIntradayReportAsync(_reportSettings, _csvSettings);
		}

		private IUnityContainer BuildContainer()
		{
			IUnityContainer container = new UnityContainer();

			container.RegisterType<ITradingDataAdapter, TradingDataAdapter>();
			container.RegisterType<IReportRepository, ReportRepository>();
			container.RegisterType<ICsvService, CsvService>();

			return container;
		}
	}
}
