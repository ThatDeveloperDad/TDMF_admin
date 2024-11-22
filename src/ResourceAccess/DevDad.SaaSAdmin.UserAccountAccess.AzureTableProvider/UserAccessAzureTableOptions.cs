using System;
using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.UserAccountAccess.AzureTableProvider;

public class UserAccessAzureTableOptions:IServiceOptions
{
    public string ConnectionString { get; set; } = string.Empty;
}
