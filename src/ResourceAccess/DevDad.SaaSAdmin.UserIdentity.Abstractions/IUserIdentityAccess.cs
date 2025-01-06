using ThatDeveloperDad.iFX.ServiceModel.Taxonomy;

namespace DevDad.SaaSAdmin.UserIdentity.Abstractions;

public interface IUserIdentityAccess : IResourceAccessService
{
    /// <summary>
    /// Identifies the Vendor that stores the user identities.
    /// </summary>
    string IdentityVendor {get;}

    Task<LoadIdentityResponse> LoadUserIdentityAsync(LoadIdentityRequest request);

    Task<ReconcileMembershipsResponse> ReconcileUserMembershipsAsync(ReconcileMembershipsRequest request);

}
