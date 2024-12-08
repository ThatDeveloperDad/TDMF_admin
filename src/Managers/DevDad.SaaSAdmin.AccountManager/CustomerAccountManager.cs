﻿using System;
using System.Threading.Tasks;
using DevDad.SaaSAdmin.AccountManager.Contracts;
using DevDad.SaaSAdmin.AccountManager.Internals;
using DevDad.SaaSAdmin.Catalog.Abstractions;
using DevDad.SaaSAdmin.RulesAccess.Abstractions;
using DevDad.SaaSAdmin.UserAccountAccess.Abstractions;
using DevDad.SaaSAdmin.UserIdentity.Abstractions;
using Microsoft.Extensions.Logging;
using ThatDeveloperDad.iFX.ServiceModel;
using ThatDeveloperDad.iFX.ServiceModel.Taxonomy;

namespace DevDad.SaaSAdmin.AccountManager
{
	public sealed class CustomerAccountManager 
	: IAccountManager
	{
		
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

		public async Task<CustomerProfileResponse> LoadOrCreateCustomerProfileAsync(CustomerProfileRequest requestData)
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

		public (CustomerProfile?, Exception?) StoreCustomerProfile(CustomerProfile profile)
		{
			_logger?.LogInformation($"StoreCustomerProfile executed.");
			return (null, null);
		}

    }
}
