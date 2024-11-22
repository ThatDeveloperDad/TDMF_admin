using DevDad.SaaSAdmin.UserAccountAccess.Abstractions;
using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.UserAccountAccess.AzureTableProvider;

internal class UserAccountAzureTableProvider
    : ServiceBase
    , IUserAccountAccess
{

    

    public async Task<UserAccountResource?> LoadUserAccountAsync(string accountId)
    {
        Console.WriteLine($"{this.GetType().Name} - LoadUserAccount {accountId}"); 
        return new UserAccountResource{UserId = accountId}; 
    }

    public async Task<UserAccountResource?> SaveUserAccountAsync(UserAccountResource userAccount)
    {
        Console.WriteLine($"{this.GetType().Name} - LoadUserAccount {userAccount.UserId}");
        return userAccount;
    }
}
