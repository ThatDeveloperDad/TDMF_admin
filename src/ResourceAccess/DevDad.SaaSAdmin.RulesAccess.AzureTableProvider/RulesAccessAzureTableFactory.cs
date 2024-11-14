using DevDad.SaaSAdmin.RulesAccess.Abstractions;
using Microsoft.Extensions.Configuration;
using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.RulesAccess.AzureTableProvider
{
	public class RulesAccessAzureTableFactory : IServiceFactory<IRulesAccess>
	{
		public IRulesAccess CreateService(IConfiguration config)
		{
			IRulesAccess service = ConfigureService(config);
			var proxy = new RulesTableInProcProxy(service);

			return proxy;
		}

		private IRulesAccess ConfigureService(IConfiguration config)
		{

			TableProviderOptions options = new();
			config.Bind(options);

			var service = new AzureTableProvider();
			service.SetConfiguration(options);

			return service;
		}
	}
}
