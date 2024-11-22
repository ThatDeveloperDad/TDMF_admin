using System;
using DevDad.SaaSAdmin.UserAccountAccess.Abstractions;
using Microsoft.Extensions.Configuration;
using ThatDeveloperDad.iFX;
using ThatDeveloperDad.iFX.ServiceModel;
using ThatDeveloperDad.iFX.Utilities;

namespace DevDad.SaaSAdmin.UserAccountAccess.AzureTableProvider;

public class UserAccessAzureTableFactory
    : ServiceFactoryBase<IUserAccountAccess, UserAccessAzureTableOptions>
{
    public override IUserAccountAccess CreateService(
        IConfiguration config, 
        IServiceProvider? standardDependencies = null)
    {
        UserAccountAzureTableProvider concreteSvc = 
            CreateServiceInstance<UserAccountAzureTableProvider>(config, false, standardDependencies);

        if (standardDependencies != null)
        {
            concreteSvc = AddStandardDependencies(concreteSvc, standardDependencies);
        }

        var proxy = new UserAccountProxyBuilder()
            .AddBehavior(LoggingBehavior<UserAccountAzureTableProvider>
                            .Create(standardDependencies))
            .CreateProxy(concreteSvc);

        return proxy;
    }

    protected override IEnumerable<Type> GetComponentDependencyList()
    {
        return Array.Empty<Type>();
    }
}
