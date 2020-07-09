﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TradingReports.Core.Helpers;

namespace TradingReports.Tools.Reporting
{
	public static class ReportHelper
	{
		public static readonly ReportSettings Settings;


		static ReportHelper()
		{
			Settings = new ReportSettings
			{
				FileNameTemplate = ConfigurationManager.AppSettings["ReportSettings:FileNameTemplate"],
				FileTimestampFormat = ConfigurationManager.AppSettings["ReportSettings:FileTimestampFormat"],
				FilesDirectory = ConfigurationManager.AppSettings["ReportSettings:FilesDirectory"],
				ReportingIntervalInMinutes = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["ReportSettings:ReportingIntervalInMinutes"])
				 ? int.Parse(ConfigurationManager.AppSettings["ReportSettings:ReportingIntervalInMinutes"])
				 : 0
			};

			// set default values
			if (string.IsNullOrEmpty(Settings.FileNameTemplate))
			{
				Settings.FileNameTemplate = "PowerPosition_{0}.csv";
			}
			if (string.IsNullOrEmpty(Settings.FileTimestampFormat))
			{
				Settings.FileTimestampFormat = "yyyyMMdd_HHmm";
			}
			if (string.IsNullOrEmpty(Settings.FilesDirectory))
			{
				Settings.FilesDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			}
			if (Settings.ReportingIntervalInMinutes == 0)
			{
				Settings.ReportingIntervalInMinutes = 5;
			}
		}


		public static string GenerateReportFilePath(DateTime utcReportDate)
		{
			DateTime localReportDate = utcReportDate.FromUtcToGmt(); // convert to UK time
			string fileName = string.Format(ReportHelper.Settings.FileNameTemplate,
				localReportDate.ToString(ReportHelper.Settings.FileTimestampFormat));

			// check file name for unique
			int fileVersion = 1;
			string fileNamePostfix = string.Empty;
			while (File.Exists(Path.Combine(ReportHelper.Settings.FilesDirectory, $"{fileName}{fileNamePostfix}")))
			{
				fileVersion++;
				fileNamePostfix = $"({fileVersion})";
			}

			string filePath = Path.Combine(ReportHelper.Settings.FilesDirectory, $"{fileName}{fileNamePostfix}");

			return filePath;
		}
	}
}