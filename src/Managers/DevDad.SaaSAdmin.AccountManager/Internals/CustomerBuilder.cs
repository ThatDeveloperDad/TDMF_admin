using System;
using System.Xml.XPath;
using DevDad.SaaSAdmin.AccountManager.Contracts;
using DevDad.SaaSAdmin.UserAccountAccess.Abstractions;
using DevDad.SaaSAdmin.UserIdentity.Abstractions;
using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.AccountManager.Internals;

internal class CustomerBuilder
{
     private readonly IUserIdentityAccess _identityAccess;
    private readonly IUserAccountAccess _accountAccess; 

    public CustomerBuilder(
        IUserAccountAccess accountAccess,
        IUserIdentityAccess identityAccess)
    {
        _accountAccess = accountAccess;
        _identityAccess = identityAccess;
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
            response.Payload = profile.ApplyIdentityFrom(loadIdentityResponse.Payload!);
            return response;
        }
        
        response.AddErrors(loadIdentityResponse);
        return response;
    }

    private async Task<CustomerProfileResponse> FetchAndApplyAccountInformation(CustomerProfileResponse response)
    {
        if(response.Payload == null)
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

        var account = await _accountAccess.LoadUserAccountAsync(response.Payload.UserId);
        if(account == null)
        {
            // Once the LoadUserAccountAsync method is return a Response object, we can simplify this.
            response.AddError(new ServiceError{
                Site = $"{nameof(CustomerBuilder)}.{nameof(FetchAndApplyAccountInformation)}",
                ErrorKind = "UserAccountNotFound",
                Message = $"Could not locate an account for user id {response.Payload.UserId}",
                Severity = ErrorSeverity.Warning
            });

            return response;
        }

        // apply whatever comes in on the account object to the response Payload.

        return response;
    }

}
