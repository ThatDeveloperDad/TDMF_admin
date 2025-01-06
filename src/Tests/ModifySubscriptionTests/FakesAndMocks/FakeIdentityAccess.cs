using System;
using DevDad.SaaSAdmin.UserIdentity.Abstractions;
using ThatDeveloperDad.iFX.ServiceModel;
using Consts = ModifySubscriptionTests.FakesAndMocks.TestConstants;

namespace ModifySubscriptionTests.FakesAndMocks;

public class FakeIdentityAccess : IUserIdentityAccess
{
    private Dictionary<string, UserIdentityResource?> _identities = new Dictionary<string, UserIdentityResource?>();
    public string IdentityVendor => Consts.FakeIdentityVendor;

    public FakeIdentityAccess()
    {
        PopulateFakeIdentities();
    }

    public Task<LoadIdentityResponse> LoadUserIdentityAsync(LoadIdentityRequest request)
    {
        LoadIdentityResponse response = new LoadIdentityResponse(request);
        bool foundit = _identities.TryGetValue(request.UserId!, out UserIdentityResource? identity);
        if(foundit == false || identity == null)
        {
            response.AddError(new ServiceError()
            {
                Message="Identity not found",
                Severity = ErrorSeverity.Error,
                ErrorKind = IdentityServiceConstants.ErrorKinds.UnknownIdentity
            });
            
        }

        response.Payload = identity;
        return Task.FromResult(response);
    }

    /// <summary>
    /// We're not testing failures of this method yet, so we're going to 
    /// return a successful response with no changes.
    /// 
    /// If we get to a place where we need to test failure handling on this operation,
    /// we can vary the output based on the request User.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public Task<ReconcileMembershipsResponse> ReconcileUserMembershipsAsync(ReconcileMembershipsRequest request)
    {
        ReconcileMembershipResult result = new();
        result.Completed = true;
        result.MembershipsAdded = 0;
        result.MembershipsRemoved = 0;
        
        ReconcileMembershipsResponse response = new ReconcileMembershipsResponse(request, result);

        return Task.FromResult(response);
    }

    private void PopulateFakeIdentities()
    {
        _identities.Add(Consts.AccountIds.UnknownUser, null);
        _identities.Add(
            Consts.AccountIds.NewIdNoProfile, 
            new UserIdentityResource()
            {
                UserId = Consts.AccountIds.NewIdNoProfile,
                DisplayName = "New User"
            });

        _identities.Add(
            Consts.AccountIds.FreeUser, 
            new UserIdentityResource()
            {
                UserId = Consts.AccountIds.FreeUser,
                DisplayName = "Tire Kicker"
            });

        _identities.Add(
            Consts.AccountIds.ActivePaid, 
            new UserIdentityResource()
            {
                UserId = Consts.AccountIds.ActivePaid,
                DisplayName = "My Buddy"
            });

        _identities.Add(
            Consts.AccountIds.ExpiredPaid, 
            new UserIdentityResource()
            {
                UserId = Consts.AccountIds.ExpiredPaid,
                DisplayName = "Temporarily Broke Guy"
            });

        _identities.Add(
            Consts.AccountIds.CancelledPaid, 
            new UserIdentityResource()
            {
                UserId = Consts.AccountIds.CancelledPaid,
                DisplayName = "Angry McAngryface"
            });
    }
}
