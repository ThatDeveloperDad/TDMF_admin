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

        UserEntity? userEntity = null;

        var tableClient = _tableService
            .GetTableClient(UserEntity.BaseTableName);

        var userEntityResponse = await tableClient
            .GetEntityIfExistsAsync<UserEntity>(
                UserEntity.TablePartitionId, 
                accountId);
        if(userEntityResponse.GetRawResponse().Status == 200)
        {
            userEntity = userEntityResponse.Value;
        }
        else
        {
            _logger?.LogWarning($"Retrieving User Account for user id {accountId} failed: {userEntityResponse.GetRawResponse().ReasonPhrase}");
        }
        
        userAccount = userEntity?.ToResource()??null;
        
        return userAccount;
    }

    public async Task<(UserAccountResource?, ServiceError?)> SaveUserAccountAsync(UserAccountResource userAccount)
    {
        UserAccountResource? savedUserAccount = null;
        string saveStep = "Start";
        try
        {
            var tableClient = _tableService
            .GetTableClient(UserEntity.BaseTableName);

        saveStep = "Convert to Entity";
        var userEntity = userAccount.ToEntity();

        if(userEntity == null)
        {
            string warningMessage = $"User Account Resource could not be converted to UserEntity.";
            _logger?.LogError(warningMessage);
            ServiceError warning = new(){
                ErrorKind = UserAccountErrors.UserAccountResource_Conversion,
                Message = warningMessage,
                Severity = ErrorSeverity.Warning,
                Site = $"{nameof(UserAccountAzureTableProvider)}.{nameof(SaveUserAccountAsync)}",
            };

            return (userAccount, warning);
        }

        saveStep = "Save Entity to Table";
        var userEntityResponse = await tableClient
            .UpsertEntityAsync(userEntity);

        if(userEntityResponse.IsError)
        {
            string errorMessage = $"Saving User Account for user id {userAccount.UserId} failed: {userEntityResponse.Status} - {userEntityResponse.ReasonPhrase}";
            _logger?.LogError(errorMessage);

            ServiceError error = new(){
                ErrorKind = UserAccountErrors.UserAccountResource_StorageError,
                Message = errorMessage,
                Severity = ErrorSeverity.Error,
                Site = $"{nameof(UserAccountAzureTableProvider)}.{nameof(SaveUserAccountAsync)}",
            };
            return (userAccount, error);
        }
        }
        catch(Exception ex)
        {
            string errorMessage = $"Saving User Account for user id {userAccount.UserId} failed at step {saveStep}: {ex.Message}";
            _logger?.LogError(ex, errorMessage);

            ServiceError error = new(){
                ErrorKind = UserAccountErrors.UserAccountResource_StorageError,
                Message = errorMessage,
                Severity = ErrorSeverity.Error,
                Site = $"{nameof(UserAccountAzureTableProvider)}.{nameof(SaveUserAccountAsync)}",
            };
            return (userAccount, error);
        }

        savedUserAccount = await LoadUserAccountAsync(userAccount.UserId);

        return (savedUserAccount, null);
    }
}
