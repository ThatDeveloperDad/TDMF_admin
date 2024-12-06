using Azure.Data.Tables;
using DevDad.SaaSAdmin.UserAccountAccess.Abstractions;
using DevDad.SaaSAdmin.UserAccountAccess.AzureTableProvider.ObjectMapping;
using DevDad.SaaSAdmin.UserAccountAccess.AzureTableProvider.StorageModels;
using Microsoft.Extensions.Logging;
using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.UserAccountAccess.AzureTableProvider;

public class UserAccountAzureTableProvider
    : IUserAccountAccess
{
    private readonly ILogger? _logger;
    private readonly UserAccessAzureTableOptions _options;
    private readonly TableServiceClient _tableService;

    public UserAccountAzureTableProvider(UserAccessAzureTableOptions options,
        ILoggerFactory? loggerFactory)
    {
        _options = options;
        _logger = loggerFactory?.CreateLogger<UserAccountAzureTableProvider>();
    
        _tableService = new TableServiceClient(_options.ConnectionString);
    }

    public async Task<UserAccountResource?> LoadUserAccountAsync(string accountId)
    {
        UserAccountResource? userAccount = null;

        var tableClient = _tableService
            .GetTableClient(UserEntity.BaseTableName);
        var userEntity = await tableClient
            .GetEntityAsync<UserEntity>(UserEntity.TablePartitionId, accountId);

        if(userEntity != null)
        {
            userAccount = userEntity.Value.ToResource();
        }
        else
        {
            _logger?.LogInformation($"UserAccount {accountId} not found.");
        }
        
        return userAccount;
    }

    public async Task<UserAccountResource?> SaveUserAccountAsync(UserAccountResource userAccount)
    {
        Console.WriteLine($"{this.GetType().Name} - LoadUserAccount {userAccount.UserId}");
        return userAccount;
    }
}
