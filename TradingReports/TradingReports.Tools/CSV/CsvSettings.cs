
using System.Configuration;

namespace TradingReports.Tools.CSV
{
	/// <summary>
	/// Provides settings for CSV export / import.
	/// </summary>
	public class CsvSettings
	{
		public string FieldSeparator { get; protected set; }

		public string Qualifier { get; protected set; }
		
		public string TimeFormat { get; protected set; }
		
		public string NumberFormat { get; protected set; }


		public CsvSettings(string fieldSeparator, string qualifier, string timeFormat, string numberFormat)
		{
			FieldSeparator = fieldSeparator;
			Qualifier = qualifier;
			TimeFormat = timeFormat;
			NumberFormat = numberFormat;
		}

		protected CsvSettings()
		{
		}


		/// <summary>
		/// Read CSV Settings from app.config file.
		/// </summary>
		/// <returns></returns>
		public static CsvSettings ReadFromConfig()
		{
			var csvSettings = new CsvSettings
			{
				FieldSeparator = ConfigurationManager.AppSettings["CsvSettings:FieldSeparator"],
				Qualifier = ConfigurationManager.AppSettings["CsvSettings:Qualifier"],
				TimeFormat = ConfigurationManager.AppSettings["CsvSettings:TimeFormat"],
				NumberFormat = ConfigurationManager.AppSettings["CsvSettings:NumberFormat"]
			};

			// set default values
			csvSettings.FieldSeparator = !string.IsNullOrEmpty(csvSettings.FieldSeparator)
				? csvSettings.FieldSeparator
				: ",";
			csvSettings.Qualifier = csvSettings.Qualifier ?? string.Empty;
			csvSettings.TimeFormat = !string.IsNullOrEmpty(csvSettings.TimeFormat)
				? csvSettings.TimeFormat
				: "HH:mm";
			csvSettings.NumberFormat = !string.IsNullOrEmpty(csvSettings.NumberFormat)
				? csvSettings.NumberFormat
				: "0.####";

			return csvSettings;
		}
	}
}
