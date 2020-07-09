
namespace TradingReports.Tools.CSV
{
	/// <summary>
	/// Provides settings for CSV export / import.
	/// </summary>
	public class CsvSettings
	{
		public string FieldSeparator { get; set; }

		public string Qualifier { get; set; }
		
		public string TimeFormat { get; set; }
		
		public string NumberFormat { get; set; }
	}
}
