using DevDad.SaaSAdmin.AccountManager.Contracts;
using DevDad.SaaSAdmin.iFX;
using DevDad.SaaSAdmin.StoreAccess.Abstractions;
using DevDad.SaaSAdmin.StoreAccess.LsApi;
using DotNetEnv;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TestConsole.OtherTests;
using ThatDeveloperDad.iFX;
using ThatDeveloperDad.iFX.CollectionUtilities;
using ThatDeveloperDad.iFX.DomainUtilities;
using ThatDeveloperDad.iFX.Serialization;

namespace TestConsole
{
	internal class Program
	{
		static void Main(string[] args)
		{

			var bootLogger = CreateBootLogger();

			TestJsonDrill();
			//TestRequestConstruction(bootLogger);

			/* IConfiguration systemConfig = LoadSystemConfiguration(bootLogger);
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
 */
			bootLogger.LogInformation("Nothing more to do.  Imma take a nap right here.");
		}

		static void TestJsonDrill()
		{
			LsApiOptions options = new LsApiOptions();
			IStoreApiAccess storeAccess = new LemonSqueezyApiProvider(options, null, new HttpClient());

			string fakeJson = testLsAPIResponse;
			string queryPath = "$.data.attributes.url";

			fakeJson = fakeJson.Replace("'", "\"");

			string? experimentResult = JsonUtilities.GetValueAtPath(fakeJson, queryPath);

			Console.WriteLine(experimentResult);
			
		}

		static void TestRequestConstruction(ILogger logger)
		{
			LsApiOptions options = new LsApiOptions()
			{
				StoreId = 132258,
				IsTestMode = true,
				BaseUrl = "https://api.lemonsqueezy.com",
				ApiKey = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJhdWQiOiI5NGQ1OWNlZi1kYmI4LTRlYTUtYjE3OC1kMjU0MGZjZDY5MTkiLCJqdGkiOiJlMTUxNThlODY5N2E1MTE0ODI5MjZhYWI2ZmE4MzYwMmYwOGQ2NGRmMjc2NDkwMGE5MTAxNzgzN2Q0ODIwYzg4ZTlkYzNjN2E0ZDQ5ZGIwYiIsImlhdCI6MTczNjE4NTEzMi41MDQ4OTQsIm5iZiI6MTczNjE4NTEzMi41MDQ4OTYsImV4cCI6MjA1MTcxNzkzMi40NzYzOTgsInN1YiI6IjM2OTM5ODUiLCJzY29wZXMiOltdfQ.bhRVApj3DTuhYi7DwA833c3WAA0asnEGe5KKfydBHJCvF6lzF0k95_Z51rJCQ646zs5YYTEjqV2QuP9r3YdRNYFGEFdamAqHNdPT6ZeU7tAD7ssYcH-fFU-VzGv6fQsK3n3NONFQmu_FGBdNKQzdD437T30aI19eM361w7NBRuw7j1F_b3wmF4o5OEbn1f0sIHqd4U7-fFQ94UbzWbg3gZuYTHeefKvE8XJgcQ7l46rR-rXUKUsH2tC3D9Qkfg86E1WI853vH1LEqQqbs5bniEQ1dJQ_o1xi4YH1bNVuCOCLwBb7Mal0Mmb0GUrSRvAfwPPMqJSkRbLEQoUpjRRIWZ422tE38VzTpA8POVUvMqIoC0WKYHlMQBG_8qOTRzMueEQ0zvO9fqRWcKgGFj6URQBISDTP_cMCPH5pfsYCGl8ETK-hmDOuBfLah56hlU38X8bqGu_dZ5InN3nuXJHbdkVKkFoomrHbQUANRWNSQXIQ9n3otoMdCsvFr313o96Q"
			};

			IStoreApiAccess storeAccess = 
				new LemonSqueezyApiProvider(options, null, new HttpClient());

			CheckoutData checkoutData = new()
			{
				CustomerEntraId = "eb4668e2-941a-480b-b132-d9300e9e6124",
				ProductIdToPurchase = "580657"
			};

			CheckoutRequest request = new("RequestBuilderTesting", checkoutData);

			var result = storeAccess.StartCheckoutAsync(request).Result;

			if(result.Successful)
			{
				Console.WriteLine(result.CheckoutUrl);
			}
			else
			{
				string errorReport = string.Join("\n", result.ErrorReport);
				Console.WriteLine("Errors occurred.");
				Console.WriteLine(errorReport);
			}

			Console.ReadLine();

		}

		static void TestEntra()
		{
			ILogger logger = CreateBootLogger();
			IConfiguration config = LoadSystemConfiguration(logger);
			EntraAccessTests tests = new(config, logger);

			tests.RunTests();
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


		const string testLsAPIResponse = @"{
  'jsonapi': {
    'version': '1.0'
  },
  'links': {
    'self': 'https://api.lemonsqueezy.com/v1/checkouts/5e8b546c-c561-4a2c-a586-40c18bb2a195'
  },
  'data': {
    'type': 'checkouts',
    'id': '5e8b546c-c561-4a2c-a586-40c18bb2a195',
    'attributes': {
      'store_id': 1,
      'variant_id': 1,
      'custom_price': 50000,
	  'my_array' : [
		{ 'name': 'Bob' },
		{ 'name': 'Sue' },
		{ 'name': 'Joe' },
		{ 'name': 'Eva' }
	  ],
      'product_options': {
        'name': '',
        'description': '',
        'media': [],
        'redirect_url': '',
        'receipt_button_text': '',
        'receipt_link_url': '',
        'receipt_thank_you_note': '',
        'enabled_variants': [1]
      },
      'checkout_options': {
        'embed': false,
        'media': true,
        'logo': true,
        'desc': true,
        'discount': true,
        'skip_trial': false,
        'subscription_preview': true,
        'button_color': '#7047EB'
      },
      'checkout_data': {
        'email': '',
        'name': '',
        'billing_address': [],
        'tax_number': '',
        'discount_code': '',
        'custom': [],
        'variant_quantities': []
      },
      'preview': {
        'currency': 'USD',
        'currency_rate': 1,
        'subtotal': 5000,
        'discount_total': 0,
        'tax': 0,
        'total': 5000,
        'subtotal_usd': 5000,
        'discount_total_usd': 0,
        'tax_usd': 0,
        'total_usd': 5000,
        'subtotal_formatted': '$50.00',
        'discount_total_formatted': '$50.00',
        'tax_formatted': '$0.00',
        'total_formatted': '$5.00'
      },
      'expires_at': '2022-10-30T15:20:06.000000Z',
      'created_at': '2022-10-14T13:03:37.000000Z',
      'updated_at': '2022-10-14T13:03:37.000000Z',
      'test_mode': false,
      'url': 'https://my-store.lemonsqueezy.com/checkout/custom/5e8b546c-c561-4a2c-a586-40c18bb2a195?expires=1667143206&signature=8f7248ad2022ef1d4111752ae02d14f8d04332274861ca5c3589eb22b5086a5b'
    },
    'relationships': {
      'store': {
        'links': {
          'related': 'https://api.lemonsqueezy.com/v1/checkouts/5e8b546c-c561-4a2c-a586-40c18bb2a195/store',
          'self': 'https://api.lemonsqueezy.com/v1/checkouts/5e8b546c-c561-4a2c-a586-40c18bb2a195/relationships/store'
        }
      },
      'variant': {
        'links': {
          'related': 'https://api.lemonsqueezy.com/v1/checkouts/5e8b546c-c561-4a2c-a586-40c18bb2a195/variant',
          'self': 'https://api.lemonsqueezy.com/v1/checkouts/5e8b546c-c561-4a2c-a586-40c18bb2a195/relationships/variant'
        }
      }
    },
    'links': {
      'self': 'https://api.lemonsqueezy.com/v1/checkouts/5e8b546c-c561-4a2c-a586-40c18bb2a195'
    }
  }
}";

	}
}
