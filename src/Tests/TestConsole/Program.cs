using DevDad.SaaSAdmin.AccountManager;
using DevDad.SaaSAdmin.AccountManager.Contracts;
using DotNetEnv;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TestConsole
{
	internal class Program
	{
		static  void Main(string[] args)
		{
			IConfiguration systemConfig = LoadSystemConfiguration();
			IServiceProvider? services = ConfigureStandardDependencies();


			var accountManager = GetAccountManager(systemConfig, services);

			CustomerProfileRequest request = new("TestingAccountManager", "a0b66013-a5ef-462f-a812-3eb4aeacff66");

			var loadResult = accountManager.LoadOrCreateCustomerProfileAsync(request).Result;
			accountManager.ManageCustomerSubscription(new SubscriptionActionRequest());
			accountManager.StoreCustomerProfile(new CustomerProfile());

		}

		static IServiceProvider? ConfigureStandardDependencies()
		{
			ServiceCollection serviceBuilder = new();
			serviceBuilder.AddLogging(
				configure=> 
				{
					configure.AddConsole();
				}
			);
			ServiceProvider services = serviceBuilder.BuildServiceProvider();

			return services;
		}

		static IAccountManager GetAccountManager(
			IConfiguration config,
			IServiceProvider? standardDependencies = null)
		{
			var factory = new CustomerAccountManagerFactory();

			return factory.CreateService(config, standardDependencies);
		}

		private static IConfiguration LoadSystemConfiguration()
		{
			Env.Load();
			var builder = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddEnvironmentVariables();
			
			return builder.Build();
		}
	}
}
