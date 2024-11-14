using DevDad.SaaSAdmin.AccountManager.Contracts;
using DevDad.SaaSAdmin.RulesAccess.Abstractions;
using ThatDeveloperDad.iFX.ServiceModel;
using ThatDeveloperDad.iFX.ServiceModel.Taxonomy;

namespace DevDad.SaaSAdmin.AccountManager
{
	internal class CustomerAccountManager : IAccountManager
	{
		private CustomerAccountManagerOptions? _options;
		private Dictionary<Type, IService> _dependencies = new();

		private IRulesAccess _rulesAccess => Proxy<IRulesAccess>();

		internal static IEnumerable<Type> GetRequiredDependencies()
		{
			Type[] requiredDependencies 
				= new Type[] 
					{ 
						typeof(IRulesAccess) 
					};

			return requiredDependencies;
		}

		internal void SetDependency<T>(T serviceDependency) where T: IService
		{
			Type dependencyType = typeof(T);
			_dependencies[dependencyType] = serviceDependency;
		}

		internal T Proxy<T>() where T: IService
		{
			T service = (T)_dependencies[typeof(T)];
			return service;
		} 

		private string GetConfigFrag()
		{
			string configFragment = string.Empty;
			if (_options != null)
			{
				configFragment = $" with config {_options.SomeContrivedNonsense}";
			}
			return configFragment;
		}

		public (CustomerProfile?, Exception?) LoadOrCreateCustomerProfile(CustomerProfileRequest requestData)
		{
			
			_rulesAccess.LoadRules();

			Console.WriteLine($"LoadOrCreateCustomerProfile{GetConfigFrag()}");
			return (null, null);
		}

		public (CustomerSubscription?, Exception?) ManageCustomerSubscription(SubscriptionActionRequest actionRequest)
		{
			Console.WriteLine($"ManageCustomerSubscription{GetConfigFrag()}");
			return (null, null);
		}

		public (CustomerProfile?, Exception?) StoreCustomerProfile(CustomerProfile profile)
		{
			Console.WriteLine($"StoreCustomerProfile{GetConfigFrag()}");
			return (null, null);
		}

		public void SetConfiguration(IServiceOptions options)
		{
			_options = (CustomerAccountManagerOptions)options;
		}
	}
}
