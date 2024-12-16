using System;
using System.Threading.Tasks;
using ThatDeveloperDad.iFX.ServiceModel.Taxonomy;

namespace DevDad.SaaSAdmin.AccountManager.Contracts
{
	public interface IAccountManager : IManagerService
    {
        Task<CustomerProfileResponse> LoadOrCreateCustomerProfileAsync(LoadAccountProfileRequest requestData);

        Task<CustomerProfileResponse> StoreCustomerProfileAsync(SaveAccountProfileRequest request);

        (CustomerSubscription?, Exception?) ManageCustomerSubscription(SubscriptionActionRequest actionRequest);
    }
}