
namespace TradingReports.Tools.Reporting
{
	/// <summary>
	/// Provides settings for report generation.
	/// </summary>
	public class ReportSettings
	{
		public string FileNameTemplate { get; set; }

		public string FileTimestampFormat { get; set; }

		public string FilesDirectory { get; set; }

		public int ReportingIntervalInMinutes { get; set; }
	}
}
