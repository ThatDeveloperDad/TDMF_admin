using DevDad.SaaSAdmin.AccountManager;
using DevDad.SaaSAdmin.AccountManager.Contracts;
using DotNetEnv;
using Microsoft.Extensions.Configuration;

namespace TestConsole
{
	internal class Program
	{
		static void Main(string[] args)
		{
			IConfiguration systemConfig = LoadSystemConfiguration();

			var accountManager = GetAccountManager(systemConfig);

			accountManager.LoadOrCreateCustomerProfile(new CustomerProfileRequest());
			accountManager.ManageCustomerSubscription(new SubscriptionActionRequest());
			accountManager.StoreCustomerProfile(new CustomerProfile());

		}

		static IAccountManager GetAccountManager(IConfiguration config)
		{
			var factory = new CustomerAccountManagerFactory();

			return factory.CreateService(config);
		}

		private static IConfiguration LoadSystemConfiguration()
		{
			Env.Load();
			var builder = new ConfigurationBuilder()
				.AddEnvironmentVariables();
			
			return builder.Build();
		}
	}
}
