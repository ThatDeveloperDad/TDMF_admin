using System;
using System.Threading.Tasks;
using DevDad.SaaSAdmin.AccountManager.Contracts;
using DevDad.SaaSAdmin.AccountManager.Internals;
using DevDad.SaaSAdmin.Catalog.Abstractions;
using DevDad.SaaSAdmin.RulesAccess.Abstractions;
using DevDad.SaaSAdmin.UserAccountAccess.Abstractions;
using DevDad.SaaSAdmin.UserIdentity.Abstractions;
using Microsoft.Extensions.Logging;
using ThatDeveloperDad.iFX.DomainUtilities;
using ThatDeveloperDad.iFX.ServiceModel;
using ThatDeveloperDad.iFX.ServiceModel.Taxonomy;

namespace DevDad.SaaSAdmin.AccountManager
{
	public sealed class CustomerAccountManager 
	: IAccountManager
	{
	# region Dependencies
		private readonly IUserIdentityAccess _userIdentityAccess;
		private readonly IUserAccountAccess _userAccountAccess;

		private readonly ICatalogAccess _catalogAccess;
		
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
				_builderInstance = new CustomerBuilder(_userAccountAccess, _userIdentityAccess, _catalogAccess);
			}
			return _builderInstance;
		}
	#endregion Dependencies
		
		public CustomerAccountManager(ILoggerFactory? loggerFactory,
			IUserIdentityAccess userIdentityAccess,
			IUserAccountAccess userAccountAccess,
			ICatalogAccess catalogAccess)
		{
			_logger = loggerFactory?.CreateLogger<CustomerAccountManager>();
			_userIdentityAccess = userIdentityAccess;
			_userAccountAccess = userAccountAccess;
			_catalogAccess = catalogAccess;
		}

		public async Task<CustomerProfileResponse> LoadOrCreateCustomerProfileAsync(LoadAccountProfileRequest requestData)
		{
			CustomerProfileResponse response = new(requestData);

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
			_logger?.LogInformation($"ManageCustomerSubscription Executed.");
			return (null, null);
		}

		public async Task<CustomerProfileResponse> StoreCustomerProfileAsync(SaveAccountProfileRequest request)
		{
			CustomerProfileResponse response = new(request);

			CustomerProfile? profile = request.Profile;

			if(profile == null)
			{
				response.AddError(new ServiceError{
					Message = "The Profile to store was null.",
					Severity = ErrorSeverity.Error,
					Site = $"{nameof(CustomerAccountManager)}.{nameof(StoreCustomerProfileAsync)}",
					ErrorKind = "ProfileNullError"
				});
				return response;
			}

			UserAccountResource userAccount = DomainObjectMapper
				.MapEntities<CustomerProfile, UserAccountResource>(profile);

			var userAccountResponse = await _userAccountAccess.SaveUserAccountAsync(userAccount);

			if(userAccountResponse.Item2 != null)
			{
				response.AddError(new ServiceError{
					Message = userAccountResponse.Item2.Message,
					Severity = ErrorSeverity.Error,
					Site = $"{nameof(CustomerAccountManager)}.{nameof(StoreCustomerProfileAsync)}",
					ErrorKind = "UserAccountSaveError"
				});
				response.Payload = request.Payload;
				return response;
			}

			var saveResult = userAccountResponse.Item1;

			if(saveResult == null)
			{
				throw new Exception("The UserAccountAccess.SaveUserAccountAsync method returned successfully, but had a null result.");
			}

			CustomerProfile updated = DomainObjectMapper
				.MapEntities<UserAccountResource, CustomerProfile>(saveResult);

			response.Payload = updated;

			return response;
		}

    }
}
