/* using Microsoft.Extensions.Logging;
using ThatDeveloperDad.iFX.ServiceModel.Taxonomy;

namespace ThatDeveloperDad.iFX.ServiceModel
{
	public abstract class ServiceProxy<T> where T : ISystemComponent
	{
		protected T _service;

		private ILogger? _logger;

		public void AddLogger(ILoggerFactory? logFactory)
		{
			if(logFactory == null)
			{
				return;
			}

			_logger = logFactory.CreateLogger(typeof(T).Name);
		}

		protected void LogInformation(string message)
		{
			_logger?.LogInformation(message);
		}

		protected ServiceProxy(T implementation)
		{
			_service = implementation;
		}
	}

	public abstract class InProcServiceProxy<T> : ServiceProxy<T> where T : ISystemComponent
	{
		protected InProcServiceProxy(T implementation) : base(implementation)
		{
		}
	}
}
 */