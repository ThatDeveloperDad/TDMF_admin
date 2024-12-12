using System;
using System.Linq;
using System.Threading.Tasks;
using DevDad.SaaSAdmin.AccountManager.Contracts;
using DevDad.SaaSAdmin.Catalog.Abstractions;
using DevDad.SaaSAdmin.iFX;
using DevDad.SaaSAdmin.UserAccountAccess.Abstractions;
using DevDad.SaaSAdmin.UserIdentity.Abstractions;
using ThatDeveloperDad.iFX.CollectionUtilities;
using ThatDeveloperDad.iFX.CollectionUtilities.Operators;
using ThatDeveloperDad.iFX.DomainUtilities;
using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.AccountManager.Internals;

internal class CustomerBuilder
{
    private readonly IUserIdentityAccess _identityAccess;
    private readonly IUserAccountAccess _accountAccess; 
    private readonly ICatalogAccess _catalogAccess;

    public CustomerBuilder(
        IUserAccountAccess accountAccess,
        IUserIdentityAccess identityAccess,
        ICatalogAccess catalogAccess)
    {
        _accountAccess = accountAccess;
        _identityAccess = identityAccess;
        _catalogAccess = catalogAccess;
    }

    public async Task<CustomerProfileResponse> LoadOrBuildCustomer(BuildProfileRequest requestData)
    {
        //First step is to try and load the identity information for the userId in the requestData.
        CustomerProfileResponse response = await FetchAndApplyIdentity(requestData);
        if(response.HasErrors == true)
        {
            return response;
        }
        
        //Next step is to try and load any account information we have for the Profile in the response.
        response = await FetchAndApplyAccountInformation(response);
        if(response.HasWarnings == true)
        {
            // If Fetch and Apply did NOT find a local profile,
            // We can create, save, and attach it here to fulfill the 
            // name of this method.
            if(response.HasErrorKind(CustomerBuilderErrors.UserProfile_NotFound))
            {
                
                response = await CreateLocalProfile(response);
                if(response.HasErrors == true)
                {
                    return response;
                }

                response.ClearErrorKind(CustomerBuilderErrors.UserProfile_NotFound);
            }

            return response;
        }

        // If we add any further steps, we should check the response as it comes back from each step
        // and return as soon as we encounter a HALT condition.  (response.HasErrors == true)

        return response;
    }

    private async Task<CustomerProfileResponse> FetchAndApplyIdentity(BuildProfileRequest requestData)
    {
        CustomerProfileResponse response = new(requestData);
        if(requestData.UserId == null)
        {
            response.AddError(new ServiceError
            {
                Message = "LoadAndBuildCustomer operation requires the UserId as its payload.",
                Severity = ErrorSeverity.Error,
            });
            return response;
        }

        CustomerProfile? profile = null;
        LoadIdentityRequest loadIdentityRequest = new(requestData, requestData.UserId);

        var loadIdentityResponse = await _identityAccess.LoadUserIdentityAsync(loadIdentityRequest);
        if(loadIdentityResponse.Successful)
        {
            profile = DomainObjectMapper.Map<UserIdentityResource, CustomerProfile>(
                loadIdentityResponse.Payload!);   
            response.Payload = profile;
            return response;
        }
        
        response.AddErrors(loadIdentityResponse);
        return response;
    }

    private async Task<CustomerProfileResponse> FetchAndApplyAccountInformation(CustomerProfileResponse response)
    {
        var profileUnderConstruction = response.Payload;

        if(profileUnderConstruction == null)
        {
            response.AddError(new ServiceError
            {
                Message = "FetchAndApplyAccountInformation requires a CustomerProfile to operate upon.",
                Severity = ErrorSeverity.Error,
                Site = $"{nameof(CustomerBuilder)}.{nameof(FetchAndApplyAccountInformation)}",
                ErrorKind = "ArgumentNullError"
            });
            return response;
        }

        var account = await _accountAccess.LoadUserAccountAsync(profileUnderConstruction.UserId);
        if(account == null)
        {
            // Once the LoadUserAccountAsync method is return a Response object, we can simplify this.
            response.AddError(new ServiceError{
                Site = $"{nameof(CustomerBuilder)}.{nameof(FetchAndApplyAccountInformation)}",
                ErrorKind = CustomerBuilderErrors.UserProfile_NotFound,
                Message = $"Could not locate an account for user id {profileUnderConstruction.UserId}",
                Severity = ErrorSeverity.Warning
            });

            return response;
        }

        // apply whatever comes in on the account object to the response Payload.
        profileUnderConstruction.ApplyAccountFrom(account);


        // Finally, update the response payload and return.
        response.Payload = profileUnderConstruction;
        return response;
    }

    private async Task<CustomerProfileResponse> CreateLocalProfile(CustomerProfileResponse response)
    {
        // Let's not assume anything, and validate that the identity information
        // is present.

        // This method is a big old messy mess.
        //TODO:  Once it's working, let's refactor it into something a little more readable.
        var profile = response.Payload;

        if(profile == null)
        {
            response.AddError(new ServiceError
            {
                Message = "CreateLocalProfile requires a CustomerProfile to operate upon.",
                Severity = ErrorSeverity.Error,
                Site = $"{nameof(CustomerBuilder)}.{nameof(CreateLocalProfile)}",
                ErrorKind = "ArgumentNullError"
            });
            return response;
        }

        if(profile.UserId == null)
        {
            response.AddError(new ServiceError
            {
                Message = "CreateLocalProfile requires a CustomerProfile with a UserId.",
                Severity = ErrorSeverity.Error,
                Site = $"{nameof(CustomerBuilder)}.{nameof(CreateLocalProfile)}",
                ErrorKind = CustomerBuilderErrors.UserIdentity_NotFound
            });
        }

        if(string.IsNullOrWhiteSpace(profile.DisplayName) == true)
        {
            response.AddError(new ServiceError
            {
                Message = "CreateLocalProfile requires a CustomerProfile with a DisplayName.",
                Severity = ErrorSeverity.Error,
                Site = $"{nameof(CustomerBuilder)}.{nameof(CreateLocalProfile)}",
                ErrorKind = CustomerBuilderErrors.UserIdentity_NotFound
            });
        }
        
        if(response.HasErrors == true)
        {
            return response;
        }

        // If the UserId isn't added as an ExternalId yet, add it now.
        // We know that userId is the ObjectId for this user in 
        // Entra.
        if(profile.ExternalIds.Any(x=> x.IdAtVendor == profile.UserId) == false)
        {
            profile.ExternalIds.Add(
                new ExternalId()
                {
                    Vendor = ExternalServiceVendors.MsEntra,
                    IdAtVendor = profile.UserId!    // We know UserId isn't null.
                }
            );
        }

        // There isn't a paid subscription.  (This is a brand new user)
        // Create a new subscription from the Free Template.
        Filter<SubscriptionTemplateResource> freeSubFilter = new();
        string freeSubSku = SubscriptionIdentifiers.SKUS_TDMF_FREE;
        freeSubFilter.AddCriteria(
            propertyName:nameof(SubscriptionTemplateResource.SKU), 
            expectedValue: freeSubSku,
            operatorKind: OperatorKinds.Equals);

        // We need a subscription template from which to build the initial, default
        // "Subscription" that all users start with.
        var filteredCatalog = await _catalogAccess.FilterCatalogAsync(freeSubFilter);
        var freeTemplateSku = filteredCatalog.FirstOrDefault()?.SKU;
        if(string.IsNullOrWhiteSpace(freeTemplateSku) == true)
        {
            response.AddError(new ServiceError
            {
                Message = $"Could not locate a subscription template with SKU {freeSubSku}, so could not finish creating the Profile.",
                Severity = ErrorSeverity.Error,
                Site = $"{nameof(CustomerBuilder)}.{nameof(CreateLocalProfile)}",
                ErrorKind = "SubscriptionTemplateNotFound"
            });
            return response;
        }

        var freeSubSpec = await _catalogAccess.GetCatalogItemAsync(freeTemplateSku);

        // Now, we use that tempalte to create a new subcription that gets added to
        // the accountProfile.
        var subscription = freeSubSpec?.ToNewSubscription();
        if(subscription != null)
        {
            subscription.UserId = profile.UserId!;
            subscription.History.Add(
                new SubscriptionActivity(){
                    ActivityDateUTC = DateTime.UtcNow,
                    ActivityKind = SubscriptionActivity.ActivityKind_Created
                }
            );

            profile.SubscriptionStatus = subscription.CurrentStatus;
            profile.Subscription = subscription;
        }
        
        // now we need to save the profile, and finally, we can return the response.

        try
        {
            UserAccountResource? profileResource = profile.ToResourceModel();
            var accessResponse = await _accountAccess.SaveUserAccountAsync(profileResource);
            
            if(accessResponse.Item2 == null)
            {
                profile = profile.ApplyAccountFrom(accessResponse.Item1!);
            }
            else
            {
                response.AddError(accessResponse.Item2);
                return response;
            }

            profile = profile.ApplyAccountFrom(profileResource);
        }
        catch
        {
            response.AddError(new ServiceError
            {
                Message = "An error occurred while trying to save the new profile.",
                Severity = ErrorSeverity.Error,
                Site = $"{nameof(CustomerBuilder)}.{nameof(CreateLocalProfile)}",
                ErrorKind = "SaveProfileError"
            });
        }
        response.Payload = profile;
        return response;
    }

}
