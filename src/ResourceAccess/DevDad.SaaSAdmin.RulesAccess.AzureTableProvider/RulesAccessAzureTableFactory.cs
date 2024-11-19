using DevDad.SaaSAdmin.RulesAccess.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ThatDeveloperDad.iFX;
using ThatDeveloperDad.iFX.ServiceModel;
using ThatDeveloperDad.iFX.Utilities;

namespace DevDad.SaaSAdmin.RulesAccess.AzureTableProvider
{
	public class RulesAccessAzureTableFactory 
	: ServiceFactoryBase<IRulesAccess, TableProviderOptions>
	, IServiceFactory<IRulesAccess>
	{
		public override IRulesAccess CreateService(
			IConfiguration config,
			IServiceProvider? standardDependencies = null)
		{
			AzureTableProvider service = 
				CreateServiceInstance<AzureTableProvider>(config, false, standardDependencies);

			var proxy = new RulesTableProxyBuilder()
				.AddBehavior(LoggingBehavior<AzureTableProvider>.Create(standardDependencies))
				.CreateProxy(service);

			if(standardDependencies != null)
			{
				service = AddStandardDependencies(service, standardDependencies);
			}
			
			return proxy;
		}

        protected override IEnumerable<Type> GetComponentDependencyList()
        {
            return Array.Empty<Type>();
        }

        
	}
}
