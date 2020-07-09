using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradingReports.Core.Repositories
{
	public abstract class BaseRepository
	{
		// TODO: Logging support 
		public void LogError(string message, Exception ex)
		{
			// TODO: ...
		}
	}
}
