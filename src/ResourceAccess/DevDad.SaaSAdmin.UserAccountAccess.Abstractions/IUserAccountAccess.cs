using ThatDeveloperDad.iFX.ServiceModel.Taxonomy;

namespace DevDad.SaaSAdmin.UserAccountAccess.Abstractions;

public interface IUserAccountAccess: IResourceAccessService
{
    Task<UserAccountResource?> SaveUserAccountAsync(UserAccountResource userAccount);

    Task<UserAccountResource?> LoadUserAccountAsync(string accountId);
}
