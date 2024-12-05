using DevDad.SaaSAdmin.UserAccountAccess.Abstractions;
using Microsoft.Extensions.Logging;
using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.UserAccountAccess.AzureTableProvider;

public class UserAccountAzureTableProvider
    : IUserAccountAccess
{
    private readonly ILogger? _logger;
    private readonly UserAccessAzureTableOptions _options;

    public UserAccountAzureTableProvider(UserAccessAzureTableOptions options,
        ILoggerFactory? loggerFactory)
    {
        _options = options;
        _logger = loggerFactory?.CreateLogger<UserAccountAzureTableProvider>();
    }

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
