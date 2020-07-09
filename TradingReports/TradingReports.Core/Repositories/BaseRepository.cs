using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;

namespace TradingReports.Core.Repositories
{
	public abstract class BaseRepository
	{
		private ILog _logger;


		protected ILog Logger
		{
			get
			{
				if (_logger == null)
				{
					_logger = LogManager.GetLogger(this.GetType());
				}

				return _logger;
			}
		}
	}
}
