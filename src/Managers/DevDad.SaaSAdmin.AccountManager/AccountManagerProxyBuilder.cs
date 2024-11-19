using System;
using DevDad.SaaSAdmin.AccountManager.Contracts;
using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.AccountManager;

internal class AccountManagerProxyBuilder
    : DynamicServiceProxy<IAccountManager, CustomerAccountManager>
{
    
}
