using ThatDeveloperDad.iFX.ServiceModel.Taxonomy;

namespace DevDad.SaaSAdmin.AccountManager.Contracts
{
	public interface IAccountManager : IManagerService
    {
        (CustomerProfile?, Exception?) LoadOrCreateCustomerProfile(CustomerProfileRequest requestData);

        (CustomerProfile?, Exception?) StoreCustomerProfile(CustomerProfile profile);

        (CustomerSubscription?, Exception?) ManageCustomerSubscription(SubscriptionActionRequest actionRequest);
    }
}
