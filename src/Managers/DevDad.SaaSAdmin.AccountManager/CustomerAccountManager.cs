using System;
using System.Threading.Tasks;
using DevDad.SaaSAdmin.AccountManager.Contracts;
using DevDad.SaaSAdmin.AccountManager.Internals;
using DevDad.SaaSAdmin.RulesAccess.Abstractions;
using DevDad.SaaSAdmin.UserAccountAccess.Abstractions;
using DevDad.SaaSAdmin.UserIdentity.Abstractions;
using Microsoft.Extensions.Logging;
using ThatDeveloperDad.iFX.ServiceModel;
using ThatDeveloperDad.iFX.ServiceModel.Taxonomy;

namespace DevDad.SaaSAdmin.AccountManager
{
	internal sealed class CustomerAccountManager 
	: IAccountManager
	{
		private readonly IRulesAccess? _rulesAccess;
		private readonly IUserIdentityAccess? _userIdentityAccess;
		private readonly IUserAccountAccess? _userAccountAccess;
		
		private readonly ILogger? _logger;

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
		
		public CustomerAccountManager()
		{
			
		}

		public async Task<CustomerProfileResponse> LoadOrCreateCustomerProfileAsync(CustomerProfileRequest requestData)
		{
			CustomerProfileResponse response = new(requestData);

			_rulesAccess?.LoadRules();
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
			Console.WriteLine($"ManageCustomerSubscription Executed.");
			return (null, null);
		}

		public (CustomerProfile?, Exception?) StoreCustomerProfile(CustomerProfile profile)
		{
			Console.WriteLine($"StoreCustomerProfile executed.");
			return (null, null);
		}

    }
}
