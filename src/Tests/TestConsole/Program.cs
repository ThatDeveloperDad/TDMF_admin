using DevDad.SaaSAdmin.AccountManager.Contracts;
using DotNetEnv;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ThatDeveloperDad.iFX;
using ThatDeveloperDad.iFX.CollectionUtilities;
using ThatDeveloperDad.iFX.DomainUtilities;

namespace TestConsole
{
	internal class Program
	{
		static void Main(string[] args)
		{
			// Get rid of this once the Filter Class is tested.
/* 			OtherTests.FilterTests.TestFilterClass();
			return; */

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
			LoadAccountProfileRequest request = new LoadAccountProfileRequest("testLoad", myUserId);
			var response = mgr.LoadOrCreateCustomerProfileAsync(request).Result;
			var profile = response.Payload;

			Console.WriteLine(profile?.UserId);
			Console.WriteLine(profile?.DisplayName);
			Console.WriteLine(profile?.Subscription?.SKU);
			Console.WriteLine(profile?.SubscriptionStatus);

			// Just logging the DomainMapper's private cache stats here.
			// No need to do it every time, but it's handy to have around.
			//DomainObjectMapper.ReportCacheStats();

			request.Payload = geekDadUserId;
			var geekResponse = mgr.LoadOrCreateCustomerProfileAsync(request).Result;
			var geekProfile = geekResponse.Payload;

			if(geekProfile != null)
			{
				Console.WriteLine(geekProfile?.UserId);
				Console.WriteLine(geekProfile?.DisplayName);
				Console.WriteLine(geekProfile?.Subscription?.SKU);
				Console.WriteLine(geekProfile?.SubscriptionStatus);

				Console.WriteLine("Now let's change the DisplayName and save it.");
				geekProfile!.DisplayName = "Geek Dad";

				var saveProfileRequest = new SaveAccountProfileRequest("testSave", geekProfile);
				var saveResponse = mgr.StoreCustomerProfileAsync(saveProfileRequest).Result;

				if(saveResponse.Successful)
				{
					Console.WriteLine("Profile Saved Successfully.");
					Console.WriteLine(saveResponse.Payload?.DisplayName);
				}
				else
				{
					Console.WriteLine("Profile Save Failed.");
					foreach(string? error in saveResponse.ErrorReport)
					{
						
						Console.WriteLine(error);
					}
					Console.WriteLine("----------------------------");
				}
			}
			else
			{
				Console.WriteLine("Geek Profile was null.  Here are the error messages:");
				foreach(string? error in geekResponse.ErrorReport)
				{
					Console.WriteLine(error);
				}
			}

			
			// Let's see how the Caching helped (or not)
			DomainObjectMapper.ReportCacheStats();

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
