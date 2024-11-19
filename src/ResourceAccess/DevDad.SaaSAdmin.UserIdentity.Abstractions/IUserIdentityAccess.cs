using ThatDeveloperDad.iFX.ServiceModel.Taxonomy;

namespace DevDad.SaaSAdmin.UserIdentity.Abstractions;

public interface IUserIdentityAccess : IResourceAccessService
{
    /// <summary>
    /// Identifies the Vendor that stores the user identities.
    /// </summary>
    string IdentityVendor {get;}

    Task<UserIdentity?> LoadUserIdentityAsync(string userId);

}
