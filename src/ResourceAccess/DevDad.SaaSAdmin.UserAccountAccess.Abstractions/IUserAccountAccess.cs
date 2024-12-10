using ThatDeveloperDad.iFX.ServiceModel;
using ThatDeveloperDad.iFX.ServiceModel.Taxonomy;

namespace DevDad.SaaSAdmin.UserAccountAccess.Abstractions;

public interface IUserAccountAccess: IResourceAccessService
{
    Task<(UserAccountResource?, ServiceError?)> SaveUserAccountAsync(UserAccountResource userAccount);

    Task<UserAccountResource?> LoadUserAccountAsync(string accountId);
}
