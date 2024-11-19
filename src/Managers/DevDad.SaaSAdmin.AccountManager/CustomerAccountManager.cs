using DevDad.SaaSAdmin.AccountManager.Contracts;
using DevDad.SaaSAdmin.RulesAccess.Abstractions;
using DevDad.SaaSAdmin.UserIdentity.Abstractions;
using ThatDeveloperDad.iFX.ServiceModel;
using ThatDeveloperDad.iFX.ServiceModel.Taxonomy;

namespace DevDad.SaaSAdmin.AccountManager
{
	internal sealed class CustomerAccountManager 
	: ManagerBase, IAccountManager
	{
		private IRulesAccess _rulesAccess => GetProxy<IRulesAccess>();
		private IUserIdentityAccess _userIdentityAccess => GetProxy<IUserIdentityAccess>();

		private string GetConfigFrag()
		{
			string configFragment = string.Empty;
			configFragment = Options<CustomerAccountManagerOptions>()?
				.SomeContrivedNonsense
				?? throw new Exception($"The {this.GetType().Name} component has not been properly configured.");

			return configFragment;
		}

		public (CustomerProfile?, Exception?) LoadOrCreateCustomerProfile(CustomerProfileRequest requestData)
		{
			_rulesAccess.LoadRules();
			var cust = _userIdentityAccess.LoadUserIdentityAsync(requestData.UserId).Result;

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

        
    }
}
