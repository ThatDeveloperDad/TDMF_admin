using System;
using System.Threading.Tasks;
using ThatDeveloperDad.iFX.ServiceModel.Taxonomy;

namespace DevDad.SaaSAdmin.AccountManager.Contracts
{
	public interface IAccountManager : IManagerService
    {
        Task<CustomerProfileResponse> LoadOrCreateCustomerProfileAsync(CustomerProfileRequest requestData);

        (CustomerProfile?, Exception?) StoreCustomerProfile(CustomerProfile profile);

        (CustomerSubscription?, Exception?) ManageCustomerSubscription(SubscriptionActionRequest actionRequest);
    }
}
