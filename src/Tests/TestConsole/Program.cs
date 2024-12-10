using DevDad.SaaSAdmin.AccountManager.Contracts;
using DotNetEnv;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ThatDeveloperDad.iFX;
using ThatDeveloperDad.iFX.CollectionUtilities;

namespace TestConsole
{
	internal class Program
	{
		static void Main(string[] args)
		{
			// Get rid of this once the Filter Class is tested.
			OtherTests.FilterTests.TestFilterClass();
			return;

			var bootLogger = CreateBootLogger();
			IConfiguration systemConfig = LoadSystemConfiguration(bootLogger);
			IServiceProvider globalUtilities = BuildUtilityProvider(systemConfig, bootLogger);

			IServiceCollection appServicesBuilder = new ServiceCollection();
			appServicesBuilder = appServicesBuilder.AddAppArchitecture(
				systemConfig,
				globalUtilities,
				bootLogger);

			IServiceProvider appServices = appServicesBuilder
				.BuildServiceProvider();

			IAccountManager? mgr = appServices.GetService<IAccountManager>();

			if(mgr == null)
			{
				bootLogger.LogError("Account Manager could not be retrieved from appServices.");
				return;
			}

			// Test the LoadCustomerProfile method
			string myUserId = "a0b66013-a5ef-462f-a812-3eb4aeacff66";
			string geekDadUserId = "eb4668e2-941a-480b-b132-d9300e9e6124";
			CustomerProfileRequest request = new CustomerProfileRequest("testing", geekDadUserId);
			var response = mgr.LoadOrCreateCustomerProfileAsync(request).Result;
			var profile = response.Payload;

			Console.WriteLine(profile?.UserId);
			Console.WriteLine(profile?.DisplayName);
			Console.WriteLine(profile?.Subscription?.SKU);
			Console.WriteLine(profile?.SubscriptionStatus);
			
			bootLogger.LogInformation("Nothing more to do.  Imma take a nap right here.");
		}

		

		static ILogger CreateBootLogger()
		{
			ILoggerFactory loggerFactory = LoggerFactory.Create(
				builder =>
				{
					builder.AddConsole();
				}
			);

			ILogger logger = loggerFactory.CreateLogger(nameof(Program));

			logger.LogInformation("App BootLogger Created.");
			return logger;
		}

		static IServiceProvider BuildUtilityProvider(IConfiguration systemConfig,
			ILogger bootLog)
		{
			bootLog.LogInformation("Configuring Utility Provider");
			IServiceCollection serviceBuilder = new ServiceCollection();
			
			serviceBuilder = ConfigureLogging(serviceBuilder, systemConfig, bootLog);

			// When we need to add things like an IHttpClientFactory, and other
			// non-domain-specific services to the utility provider, we'll do that
			// here.

			ServiceProvider services = serviceBuilder.BuildServiceProvider();

			return services;
		}

		private static IServiceCollection ConfigureLogging(
			IServiceCollection serviceBuilder,
        	IConfiguration config,
        	ILogger? logger = null)
		{
			try
			{
				serviceBuilder.AddLogging(logBuilder =>
				{
					var logConfig = config.GetSection("Logging");
					if(logConfig != null)
					{
						logBuilder.AddConfiguration(logConfig);
					}
					logBuilder.AddConsole();
				});
				logger?.LogInformation("Global Logging Added to SharedServices.");
			}
			catch (Exception ex)
			{
				logger?.LogWarning(ex, "Global logging could not be added.  System will not log at runtime.");
			}

			return serviceBuilder;
		}

		private static IConfiguration LoadSystemConfiguration(ILogger bootLog)
		{
			#if DEBUG
			bootLog.LogInformation("Running in local debug mode.  Load custom environment variables from .env file.");
			Env.Load();
			#endif
			
			var builder = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddEnvironmentVariables();
			
			bootLog.LogInformation("Configuration Loaded.");

			return builder.Build();
		}
	}
}
