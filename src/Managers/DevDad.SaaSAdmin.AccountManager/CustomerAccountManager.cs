using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevDad.SaaSAdmin.AccountManager.Contracts;
using DevDad.SaaSAdmin.AccountManager.Internals;
using DevDad.SaaSAdmin.Catalog.Abstractions;
using DevDad.SaaSAdmin.iFX;
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
		private CustomerBuilder AccountBuilder()
		{
			if(_builderInstance == null)
			{
				if(_userAccountAccess == null || _userIdentityAccess == null)
				{
					throw new Exception("The UserAccountAccess and UserIdentityAccess dependencies have not been properly initialized.");
				}
				_builderInstance = new CustomerBuilder(_userAccountAccess, _userIdentityAccess, _catalogAccess, _logger);
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

			var builder = AccountBuilder();
			
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

		public async Task<ManageSubscriptionResponse> ManageCustomerSubscriptionAsync(ManageSubscriptionRequest actionRequest)
		{
			ManageSubscriptionResponse thisResponse = new(actionRequest, null);

			// When a SubsriptionActionRequest arrives, we'll generally follow the same basic steps:
			if(actionRequest == null)
			{
				ServiceError nullRequestPayload = new()
				{
					Message = "The SubscriptionActionRequest was null.",
					Severity = ErrorSeverity.Error,
					Site = $"{nameof(CustomerAccountManager)}.{nameof(ManageCustomerSubscriptionAsync)}",
					ErrorKind = "NullRequestPayload"
				};
				thisResponse.AddError(nullRequestPayload);
				thisResponse.Payload = false;
				return thisResponse;
			}
			
			var requestErrors = TryGetChangeDetail(actionRequest, out SubscriptionActionDetail? actionDetail);

			// now, we can interrogate the requestErrors and run a null-check on the details.
			if(requestErrors.Any())
			{
				thisResponse.AddErrors(requestErrors);
				thisResponse.Payload = false;
				return thisResponse;
			}

			if(actionDetail == null)
			{
				ServiceError nullRequestPayload = new()
				{
					Message = "The SubscriptionActionDetail is null.",
					Severity = ErrorSeverity.Error,
					Site = $"{nameof(CustomerAccountManager)}.{nameof(ManageCustomerSubscriptionAsync)}",
					ErrorKind = "NullRequestPayload"
				};
				thisResponse.AddError(nullRequestPayload);
				thisResponse.Payload = false;
				return thisResponse;
			}
			
			//  1 & 2 are the same for every request.  Do Those Here.
			// 1:  Load the Profile of the identified Customer.
			BuildProfileRequest buildProfRequest = new(actionRequest, actionRequest.CustomerProfileId);
			CustomerProfileResponse builderResponse = await AccountBuilder().LoadOrBuildCustomer(buildProfRequest);

			if(builderResponse.HasErrors)
			{
				thisResponse.AddErrors(builderResponse);
				thisResponse.Payload = false;
				return thisResponse;
			}

			CustomerProfile? customerProfile = builderResponse.Payload;
			
			// If we can neither Load not Create a profile for the identified Customer, we need to bail now.
			if(customerProfile == null)
			{
				ServiceError profileNotFound = new()
				{
					Message = $"The Customer Profile {actionRequest.CustomerProfileId} was not found.",
					Severity = ErrorSeverity.Error,
					Site = $"{nameof(CustomerAccountManager)}.{nameof(ManageCustomerSubscriptionAsync)}",
					ErrorKind = "ProfileNotFound"
				};
				thisResponse.AddError(profileNotFound);
				thisResponse.Payload = false;
				return thisResponse;
			}

			string? userIdentityId = customerProfile.GetUserIdForVendor(_userIdentityAccess.IdentityVendor);
			if(userIdentityId == null)
			{
				// Something Really, REALLY F***ed up happened to get to this edge case.
				ServiceError noUserIdentityId = new()
				{
					Message = $"The Customer Profile {actionRequest.CustomerProfileId} has no Id in the Identity Service.",
					Severity = ErrorSeverity.Error,
					Site = $"{nameof(CustomerAccountManager)}.{nameof(ManageCustomerSubscriptionAsync)}",
					ErrorKind = "NoUserIdentityId"
				};
				thisResponse.AddError(noUserIdentityId);
				thisResponse.Payload = false;
				return thisResponse;
			}

			// 2:  Load the Catalog Item that gives us the details for the Subscription SKU
			SubscriptionTemplateResource? skuTemplate = 
				await _catalogAccess.GetCatalogItemAsync(actionDetail.SubscriptionSku);
			
			// If we can't load the Template for the provided subscription SKU, we cannot continue.
			if(skuTemplate == null)
			{
				ServiceError skuNotFound = new()
				{
					Message = $"The Subscription SKU {actionDetail.SubscriptionSku} was not found in the Catalog.",
					Severity = ErrorSeverity.Error,
					Site = $"{nameof(CustomerAccountManager)}.{nameof(ManageCustomerSubscriptionAsync)}",
					ErrorKind = "SkuNotFound"
				};
				thisResponse.AddError(skuNotFound);
				return thisResponse;
			}

			//  3 & 4 are going to be different for each Activity.  We'll use a Strategy Pattern for those.
			// 3:  Make sure the Activity is applicable to the current Subscription Status
			// 4:  Perform the Activity
			ModifySubscriptionData changeSubData = new(customerProfile, actionDetail);
			ModifySubscriptionRequest changeSubscriptionRequest = new(actionRequest, changeSubData);
			var changeSubscriptionResponse = await AccountBuilder().PerformSubscriptionAction(changeSubscriptionRequest);
			
			if(changeSubscriptionResponse.HasErrors)
			{
				thisResponse.AddErrors(changeSubscriptionResponse);
				thisResponse.Payload = false;
				return thisResponse;
			}

			if(changeSubscriptionResponse.HasWarnings)
			{
				thisResponse.AddErrors(changeSubscriptionResponse);
			}

			if(changeSubscriptionResponse.Payload == null)
			{
				ServiceError noChangeResult = new()
				{
					Message = "The Change Subscription Activity did not return a result.",
					Severity = ErrorSeverity.Error,
					Site = $"{nameof(CustomerAccountManager)}.{nameof(ManageCustomerSubscriptionAsync)}",
					ErrorKind = "NoChangeResult"
				};
				thisResponse.AddError(noChangeResult);
				thisResponse.Payload = false;
				return thisResponse;
			}

			customerProfile = changeSubscriptionResponse.Payload;

			//  5, 6, and 7 are the same for every request.  Do Those Here.
			// 5:  Save the modified account back to local storage.
			UserAccountResource accountResource = DomainObjectMapper
				.MapEntities<CustomerProfile, UserAccountResource>(customerProfile);
			var saveResult = await _userAccountAccess.SaveUserAccountAsync(accountResource);
			if(saveResult.Item2 != null)
			{
				thisResponse.AddError(new ServiceError{
					Message = saveResult.Item2.Message,
					Severity = ErrorSeverity.Error,
					Site = $"{nameof(CustomerAccountManager)}.{nameof(ManageCustomerSubscriptionAsync)}",
					ErrorKind = "UserAccountSaveError"
				});
				return thisResponse;
			}

			// 6:  Reconcile the Authorization Groups that the customer SHOULD have membership in at the Identity Service.
			//This facility doesn't exist yet.  We'll need to add it to the UserIdentityAccess service.
			var reconcileData = new ReconcileMembershipsData
			{
				UserId = userIdentityId!,
				ExpectedGroups = skuTemplate.ConfersMembershipIn
			};
			
			var reconcileGroupsRequest = new ReconcileMembershipsRequest
				(actionRequest,
				 reconcileData);

			var reconcileResponse = await _userIdentityAccess.ReconcileUserMembershipsAsync(reconcileGroupsRequest);
			if(reconcileResponse.HasErrors)
			{
				thisResponse.AddErrors(reconcileResponse);
				thisResponse.Payload = false;
			}

			string reconcileLog = $"Reconciled User Memberships for {userIdentityId} in {actionRequest.WorkloadName}: Added {reconcileResponse.MembershipsAdded}, Removed {reconcileResponse.MembershipsRemoved}";
			_logger?.LogInformation(reconcileLog);

			// 7:  Handle the result of all this stuff.
			if(thisResponse.HasErrors)
			{
				thisResponse.Payload = false;
				// Need a way to send an alert to a Human here.
			}
			else
			{
				thisResponse.Payload = true;
			}

			return thisResponse;
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

		private List<ServiceError> TryGetChangeDetail
			(ManageSubscriptionRequest request, 
			 out SubscriptionActionDetail? detail)
		{
			List<ServiceError> errors = new();
			SubscriptionActionDetail? actionDetail = request.RequestDetail;

			// If the Request Details aren't present, add an error and bail.
			// We can't even validate the rest of the things without it.
			if(actionDetail == null)
			{
				ServiceError malformedRequest = new()
				{
					Message = "The Required SubscriptionActionDetail on the request was null.",
					Severity = ErrorSeverity.Error,
					Site = $"{nameof(CustomerAccountManager)}.{nameof(ManageCustomerSubscriptionAsync)}",
					ErrorKind = "MissingRequestPayload"
				};
				errors.Add(malformedRequest);
				detail = null;
				return errors;
			}

			// We know the Details exist, so we can validate those next.
			// Rather than bail for each error, we'll accumulate them.
			actionDetail = request.RequestDetail!;

			// Customer's ProfileId must be present.
			if(string.IsNullOrWhiteSpace(actionDetail.CustomerProfileId) == true)
			{
				ServiceError malformedRequest = new()
				{
					Message = "The CustomerProfileId on the request payload was null or empty.",
					Severity = ErrorSeverity.Error,
					Site = $"{nameof(CustomerAccountManager)}.{nameof(ManageCustomerSubscriptionAsync)}",
					ErrorKind = "MalformedRequestPayload"
				};
				errors.Add(malformedRequest);
			}
			
			// The Sku for the subcription must be present.
			if(string.IsNullOrWhiteSpace(actionDetail.SubscriptionSku) == true)
			{
				// we can't really take any further action.
				ServiceError malformedRequest = new()
				{
					Message = "The request detail did not identify the Type of Subscription to be modified. (SubscriptionSku was missing).",
					Severity = ErrorSeverity.Error,
					Site = $"{nameof(CustomerAccountManager)}.{nameof(ManageCustomerSubscriptionAsync)}",
					ErrorKind = "MalformedRequestPayload"
				};
				errors.Add(malformedRequest);
			}

			// The Activity must be both present and well-known.
			if(SubscriptionChangeKinds.AllowedValues.Contains(actionDetail.ActionName) == false)
			{
				ServiceError unknownActivity = new()
				{
					Message = $"{actionDetail.ActionName} is not a known Subscription Activity.",
					Severity = ErrorSeverity.Error,
					Site = $"{nameof(CustomerAccountManager)}.{nameof(ManageCustomerSubscriptionAsync)}",
					ErrorKind = "UnknownActivity"
				};
				errors.Add(unknownActivity);
			}


			detail = actionDetail;
			return errors;
		}

    }
}
