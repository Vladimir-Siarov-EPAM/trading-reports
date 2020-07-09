
using System.Configuration;
using System.IO;
using System.Reflection;

namespace TradingReports.Tools.Reporting
{
	/// <summary>
	/// Provides settings for report generation.
	/// </summary>
	public class ReportSettings
	{
		public string FileNameTemplate { get; protected set; }

		public string FileTimestampFormat { get; protected set; }

		public string FilesDirectory { get; protected set; }

		public int ReportingIntervalInMinutes { get; protected set; }


		public ReportSettings(string fileNameTemplate, string fileTimestampFormat, string filesDirectory,
			int reportingIntervalInMinutes)
		{
			FileNameTemplate = fileNameTemplate;
			FileTimestampFormat = fileTimestampFormat;
			FilesDirectory = filesDirectory;
			ReportingIntervalInMinutes = reportingIntervalInMinutes;
		}

		protected ReportSettings()
		{
		}


		/// <summary>
		/// Read Report Settings from app.config file.
		/// </summary>
		/// <returns></returns>
		public static ReportSettings ReadFromConfig()
		{
			var settings = new ReportSettings
			{
				FileNameTemplate = ConfigurationManager.AppSettings["ReportSettings:FileNameTemplate"],
				FileTimestampFormat = ConfigurationManager.AppSettings["ReportSettings:FileTimestampFormat"],
				FilesDirectory = ConfigurationManager.AppSettings["ReportSettings:FilesDirectory"],
				ReportingIntervalInMinutes = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["ReportSettings:ReportingIntervalInMinutes"])
					? int.Parse(ConfigurationManager.AppSettings["ReportSettings:ReportingIntervalInMinutes"])
					: 0
			};

			// set default values
			if (string.IsNullOrEmpty(settings.FileNameTemplate))
			{
				settings.FileNameTemplate = "PowerPosition_{0}.csv";
			}
			if (string.IsNullOrEmpty(settings.FileTimestampFormat))
			{
				settings.FileTimestampFormat = "yyyyMMdd_HHmm";
			}
			if (string.IsNullOrEmpty(settings.FilesDirectory))
			{
				settings.FilesDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			}
			if (settings.ReportingIntervalInMinutes == 0)
			{
				settings.ReportingIntervalInMinutes = 5;
			}

			return settings;
		}
	}
}
