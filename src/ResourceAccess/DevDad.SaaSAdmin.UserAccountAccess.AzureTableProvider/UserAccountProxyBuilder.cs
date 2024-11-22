using System;
using DevDad.SaaSAdmin.UserAccountAccess.Abstractions;
using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.UserAccountAccess.AzureTableProvider;

internal class UserAccountProxyBuilder
    : DynamicServiceProxy<IUserAccountAccess, UserAccountAzureTableProvider>
{

}
