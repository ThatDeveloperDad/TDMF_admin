using DevDad.SaaSAdmin.AccountManager.Contracts;
using DevDad.SaaSAdmin.AccountManager.Internals;
using DevDad.SaaSAdmin.RulesAccess.Abstractions;
using DevDad.SaaSAdmin.UserAccountAccess.Abstractions;
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
		private IUserAccountAccess _userAccountAccess => GetProxy<IUserAccountAccess>();
		
		private CustomerBuilder? _builderInstance;
		private CustomerBuilder GetAccountBuilder()
		{
			if(_builderInstance == null)
			{
				if(_userAccountAccess == null || _userIdentityAccess == null)
				{
					throw new Exception("The UserAccountAccess and UserIdentityAccess dependencies have not been properly initialized.");
				}
				_builderInstance = new CustomerBuilder(_userAccountAccess, _userIdentityAccess);
			}
			return _builderInstance;
		}
		
		//TODO:  This can go away once the rest of the Manager methods have been implemented.
		private string GetConfigFrag()
		{
			string configFragment = string.Empty;
			configFragment = Options<CustomerAccountManagerOptions>()?
				.SomeContrivedNonsense
				?? throw new Exception($"The {this.GetType().Name} component has not been properly configured.");

			return configFragment;
		}

		public async Task<CustomerProfileResponse> LoadOrCreateCustomerProfileAsync(CustomerProfileRequest requestData)
		{
			CustomerProfileResponse response = new(requestData);

			_rulesAccess.LoadRules();
			var builder = GetAccountBuilder();
			
			BuildProfileRequest builderRequest = new(requestData, requestData.UserId);

			var builderResponse = await builder.LoadOrBuildCustomer(builderRequest);
			
			if(builderResponse.HasErrors)
			{
				response.AddErrors(builderResponse);
				return response;
			}

			response.Payload = builderResponse.Payload;

			if(response.Payload == null)
			{
				response.AddError(new ServiceError{
					Message = $"A Profile could not be loaded or created for user id {requestData.UserId}",
					Severity = ErrorSeverity.Error,
					Site = $"{nameof(CustomerAccountManager)}.{nameof(LoadOrCreateCustomerProfileAsync)}",
					ErrorKind = "ProfileLoadError"
					});
			}

			return response;
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
