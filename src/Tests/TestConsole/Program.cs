using System.Net.Http.Json;
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

			//TestJsonDrill();
			//TestRequestConstruction(bootLogger);
			TestWebhookProcessor(bootLogger);

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

		static void TestWebhookProcessor(ILogger logger)
		{
			string createSubPayload = testLsSubCreateCall.Replace("'", "\"");
			string updateSubPayload = testLsSubUpdateCall.Replace("'", "\"");
			string url = "http://localhost:5083/hooks/processStoreEvent";

			var lsCreateSubData = new
			{
				EventJson = createSubPayload,
				RequestId = Guid.NewGuid().ToString()
			};

			var lsUpdateSubData = new
			{
				EventJson = updateSubPayload,
				RequestId = Guid.NewGuid().ToString()
			};

			HttpClient apiClient = new HttpClient();
			
			var createResponse = apiClient.PostAsJsonAsync<object>(url, lsCreateSubData).Result;
			
			var createResponseContent = createResponse.Content.ReadAsStringAsync().Result;
			Console.WriteLine($"Create: ResponseStatus: {createResponse.StatusCode}");
			Console.WriteLine($"Create Content: {createResponseContent}");

			var updateResponse = apiClient.PostAsJsonAsync<object>(url, lsUpdateSubData).Result;

			var updateRespContent = updateResponse.Content.ReadAsStringAsync().Result;
			Console.WriteLine($"Update: ResponseStatus: {updateResponse.StatusCode}");
			Console.WriteLine($"Update Content: {updateRespContent}");
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
				CustomerEntraId = "a996d415-159c-47fa-ae5a-4b6db581ebb1",
				ProductIdToPurchase = "580657",
				LocalSystemProductSku = SubscriptionIdentifiers.SKUS_TDMF_PAID_MONTHLY
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

		const string testLsSubCreateCall = @"{
    'data': {
      'id': '899891',
      'type': 'subscriptions',
      'links': {
        'self': 'https://api.lemonsqueezy.com/v1/subscriptions/899891'
      },
      'attributes': {
        'urls': {
          'customer_portal': 'https://store.the-dms-familiar.com/billing?expires=1736891709&test_mode=1&user=4201939&signature=31a43855eeca4c1bb27d1115fcb25a5183c0c15a42e47bb7085cb0a8c34c956f',
          'update_payment_method': 'https://store.the-dms-familiar.com/subscription/899891/payment-details?expires=1736891709&signature=155b0b9d1c89e31549203319eec3170147ee660b3d4c3d17b99782439c3e1b26',
          'customer_portal_update_subscription': 'https://store.the-dms-familiar.com/billing/899891/update?expires=1736891709&user=4201939&signature=46e84bb125a81d2468d84793cd7c3bda21244cc89cf46a6e997dcdebd45ffdf8'
        },
        'pause': null,
        'status': 'active',
        'ends_at': null,
        'order_id': 4584390,
        'store_id': 132258,
        'cancelled': false,
        'renews_at': '2025-02-14T15:55:02.000000Z',
        'test_mode': true,
        'user_name': 'Robbie McTesterface',
        'card_brand': 'visa',
        'created_at': '2025-01-14T15:55:03.000000Z',
        'product_id': 383462,
        'updated_at': '2025-01-14T15:55:08.000000Z',
        'user_email': 'istestingstuffrob@gmail.com',
        'variant_id': 580657,
        'customer_id': 4747148,
        'product_name': 'The DMs Familiar - Monthly Access',
        'variant_name': 'Default',
        'order_item_id': 4525703,
        'trial_ends_at': null,
        'billing_anchor': 14,
        'card_last_four': '4242',
        'status_formatted': 'Active',
        'first_subscription_item': {
          'id': 684564,
          'price_id': 870358,
          'quantity': 1,
          'created_at': '2025-01-14T15:55:09.000000Z',
          'updated_at': '2025-01-14T15:55:09.000000Z',
          'is_usage_based': false,
          'subscription_id': 899891
        }
      },
      'relationships': {
        'order': {
          'links': {
            'self': 'https://api.lemonsqueezy.com/v1/subscriptions/899891/relationships/order',
            'related': 'https://api.lemonsqueezy.com/v1/subscriptions/899891/order'
          }
        },
        'store': {
          'links': {
            'self': 'https://api.lemonsqueezy.com/v1/subscriptions/899891/relationships/store',
            'related': 'https://api.lemonsqueezy.com/v1/subscriptions/899891/store'
          }
        },
        'product': {
          'links': {
            'self': 'https://api.lemonsqueezy.com/v1/subscriptions/899891/relationships/product',
            'related': 'https://api.lemonsqueezy.com/v1/subscriptions/899891/product'
          }
        },
        'variant': {
          'links': {
            'self': 'https://api.lemonsqueezy.com/v1/subscriptions/899891/relationships/variant',
            'related': 'https://api.lemonsqueezy.com/v1/subscriptions/899891/variant'
          }
        },
        'customer': {
          'links': {
            'self': 'https://api.lemonsqueezy.com/v1/subscriptions/899891/relationships/customer',
            'related': 'https://api.lemonsqueezy.com/v1/subscriptions/899891/customer'
          }
        },
        'order-item': {
          'links': {
            'self': 'https://api.lemonsqueezy.com/v1/subscriptions/899891/relationships/order-item',
            'related': 'https://api.lemonsqueezy.com/v1/subscriptions/899891/order-item'
          }
        },
        'subscription-items': {
          'links': {
            'self': 'https://api.lemonsqueezy.com/v1/subscriptions/899891/relationships/subscription-items',
            'related': 'https://api.lemonsqueezy.com/v1/subscriptions/899891/subscription-items'
          }
        },
        'subscription-invoices': {
          'links': {
            'self': 'https://api.lemonsqueezy.com/v1/subscriptions/899891/relationships/subscription-invoices',
            'related': 'https://api.lemonsqueezy.com/v1/subscriptions/899891/subscription-invoices'
          }
        }
      }
    },
    'meta': {
      'test_mode': true,
      'event_name': 'subscription_created',
      'webhook_id': '8fa6746e-6478-4999-b947-ff264c1a699d',
      'custom_data': {
        'local_sku': 'DM-FAMILIAR-PD-MONTHLY',
        'user_identity_id': 'a996d415-159c-47fa-ae5a-4b6db581ebb1'
      }
    }
  }";

		const string testLsSubUpdateCall = @"{
  'data': {
    'id': '899891',
    'type': 'subscriptions',
    'links': {
      'self': 'https://api.lemonsqueezy.com/v1/subscriptions/899891'
    },
    'attributes': {
      'urls': {
        'customer_portal': 'https://store.the-dms-familiar.com/billing?expires=1736891738&test_mode=1&user=4201939&signature=657404c2776576fe79cf142c3d35cad1f0e9ec68c9a3add06104ed226f5e7ef1',
        'update_payment_method': 'https://store.the-dms-familiar.com/subscription/899891/payment-details?expires=1736891738&signature=cc093516622a7984b0ee21e9ee6070587157f51ec8a4d5c67ddf55115d867660',
        'customer_portal_update_subscription': 'https://store.the-dms-familiar.com/billing/899891/update?expires=1736891738&user=4201939&signature=9a3a6f6b488cbe198456f8f5e42d967c472f33c7a79caa7e8a3f7f9089fbf83a'
      },
      'pause': null,
      'status': 'active',
      'ends_at': null,
      'order_id': 4584390,
      'store_id': 132258,
      'cancelled': false,
      'renews_at': '2025-02-14T15:55:02.000000Z',
      'test_mode': true,
      'user_name': 'Robbie McTesterface',
      'card_brand': 'visa',
      'created_at': '2025-01-14T15:55:03.000000Z',
      'product_id': 383462,
      'updated_at': '2025-01-14T15:55:08.000000Z',
      'user_email': 'istestingstuffrob@gmail.com',
      'variant_id': 580657,
      'customer_id': 4747148,
      'product_name': 'The DMs Familiar - Monthly Access',
      'variant_name': 'Default',
      'order_item_id': 4525703,
      'trial_ends_at': null,
      'billing_anchor': 14,
      'card_last_four': '4242',
      'status_formatted': 'Active',
      'first_subscription_item': {
        'id': 684564,
        'price_id': 870358,
        'quantity': 1,
        'created_at': '2025-01-14T15:55:09.000000Z',
        'updated_at': '2025-01-14T15:55:09.000000Z',
        'is_usage_based': false,
        'subscription_id': 899891
      }
    },
    'relationships': {
      'order': {
        'links': {
          'self': 'https://api.lemonsqueezy.com/v1/subscriptions/899891/relationships/order',
          'related': 'https://api.lemonsqueezy.com/v1/subscriptions/899891/order'
        }
      },
      'store': {
        'links': {
          'self': 'https://api.lemonsqueezy.com/v1/subscriptions/899891/relationships/store',
          'related': 'https://api.lemonsqueezy.com/v1/subscriptions/899891/store'
        }
      },
      'product': {
        'links': {
          'self': 'https://api.lemonsqueezy.com/v1/subscriptions/899891/relationships/product',
          'related': 'https://api.lemonsqueezy.com/v1/subscriptions/899891/product'
        }
      },
      'variant': {
        'links': {
          'self': 'https://api.lemonsqueezy.com/v1/subscriptions/899891/relationships/variant',
          'related': 'https://api.lemonsqueezy.com/v1/subscriptions/899891/variant'
        }
      },
      'customer': {
        'links': {
          'self': 'https://api.lemonsqueezy.com/v1/subscriptions/899891/relationships/customer',
          'related': 'https://api.lemonsqueezy.com/v1/subscriptions/899891/customer'
        }
      },
      'order-item': {
        'links': {
          'self': 'https://api.lemonsqueezy.com/v1/subscriptions/899891/relationships/order-item',
          'related': 'https://api.lemonsqueezy.com/v1/subscriptions/899891/order-item'
        }
      },
      'subscription-items': {
        'links': {
          'self': 'https://api.lemonsqueezy.com/v1/subscriptions/899891/relationships/subscription-items',
          'related': 'https://api.lemonsqueezy.com/v1/subscriptions/899891/subscription-items'
        }
      },
      'subscription-invoices': {
        'links': {
          'self': 'https://api.lemonsqueezy.com/v1/subscriptions/899891/relationships/subscription-invoices',
          'related': 'https://api.lemonsqueezy.com/v1/subscriptions/899891/subscription-invoices'
        }
      }
    }
  },
  'meta': {
    'test_mode': true,
    'event_name': 'subscription_updated',
    'webhook_id': 'b6012386-41d9-441d-a970-38c6d6278ecb',
    'custom_data': {
      'local_sku': 'DM-FAMILIAR-PD-MONTHLY',
      'user_identity_id': 'a996d415-159c-47fa-ae5a-4b6db581ebb1'
    }
  }
}";
	}
}
