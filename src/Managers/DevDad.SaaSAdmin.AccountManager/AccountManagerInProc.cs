using DevDad.SaaSAdmin.AccountManager.Contracts;
using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.AccountManager
{
	internal class AccountManagerInProc
		: InProcServiceProxy<IAccountManager>,
		  IAccountManager
	{
		public AccountManagerInProc(IAccountManager implementation) : base(implementation)
		{
		}

		public (CustomerProfile?, Exception?) LoadOrCreateCustomerProfile(CustomerProfileRequest requestData)
		{
			return _service.LoadOrCreateCustomerProfile(requestData);
		}

		public (CustomerSubscription?, Exception?) ManageCustomerSubscription(SubscriptionActionRequest actionRequest)
		{
			return _service.ManageCustomerSubscription(actionRequest);
		}

		public void SetConfiguration(IServiceOptions options)
		{
			throw new NotImplementedException();
		}

		public (CustomerProfile?, Exception?) StoreCustomerProfile(CustomerProfile profile)
		{
			return _service.StoreCustomerProfile(profile);
		}
	}
}
