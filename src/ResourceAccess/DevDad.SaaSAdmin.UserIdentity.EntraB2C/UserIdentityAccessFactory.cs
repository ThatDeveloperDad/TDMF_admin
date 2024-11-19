using DevDad.SaaSAdmin.UserIdentity.Abstractions;
using Microsoft.Extensions.Configuration;
using ThatDeveloperDad.iFX;
using ThatDeveloperDad.iFX.ServiceModel;
using ThatDeveloperDad.iFX.Utilities;

namespace DevDad.SaaSAdmin.UserIdentity.EntraB2C;

public sealed class UserIdentityAccessFactory 
    : ServiceFactoryBase<IUserIdentityAccess, MsGraphOptions>
{
    public override IUserIdentityAccess CreateService
        (IConfiguration config
        , IServiceProvider? standardDependencies = null)
    {
        
        UserAccessEntraProvider concreteSvc = 
            CreateServiceInstance<UserAccessEntraProvider>(config, false, standardDependencies);
        
        

        if(standardDependencies != null)
        {
            concreteSvc = AddStandardDependencies(concreteSvc, standardDependencies);
        }

        var proxy = new UserAccessProxyBuilder()
            .AddBehavior(LoggingBehavior<UserAccessEntraProvider>.Create(standardDependencies))
            .CreateProxy(concreteSvc);
        return proxy;
    }

    protected override IEnumerable<Type> GetComponentDependencyList()
    {
        return Array.Empty<Type>();
    }

}
