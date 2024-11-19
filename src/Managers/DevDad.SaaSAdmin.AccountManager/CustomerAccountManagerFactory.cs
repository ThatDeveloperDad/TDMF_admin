using ThatDeveloperDad.iFX.ServiceModel;
using DevDad.SaaSAdmin.AccountManager.Contracts;
using Microsoft.Extensions.Configuration;
using ThatDeveloperDad.iFX;
using DevDad.SaaSAdmin.RulesAccess.Abstractions;
using DevDad.SaaSAdmin.UserIdentity.Abstractions;
using ThatDeveloperDad.iFX.Utilities;

namespace DevDad.SaaSAdmin.AccountManager
{
	public sealed class CustomerAccountManagerFactory 
		: ServiceFactoryBase<IAccountManager, CustomerAccountManagerOptions>
		, IServiceFactory<IAccountManager>
	{
		public override IAccountManager CreateService(
			IConfiguration config
			, IServiceProvider? standardDependencies = null)
		{
			 CustomerAccountManager service 
				= CreateServiceInstance<CustomerAccountManager>(config, true, standardDependencies);
			
			//var proxy = new AccountManagerInProc(service);
			var proxy = new AccountManagerProxyBuilder()
				.AddBehavior(LoggingBehavior<CustomerAccountManager>.Create(standardDependencies))
				.CreateProxy(service);

			if(standardDependencies != null)
			{
				service = AddStandardDependencies(service, standardDependencies);
				//dynamicProxy = AddStandardDependenciesToProxy(dynamicProxy, standardDependencies);
			}
			
			return proxy;
		}

        protected override IEnumerable<Type> GetComponentDependencyList()
        {
            var deps = new List<Type>
			{
				typeof(IRulesAccess),
				typeof(IUserIdentityAccess)
			};
			return deps;
        }
	}
}
