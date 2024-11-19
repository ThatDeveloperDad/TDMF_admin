using System;
using DevDad.SaaSAdmin.UserIdentity.Abstractions;
using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.UserIdentity.EntraB2C;

internal class UserAccessProxyBuilder
    : DynamicServiceProxy<IUserIdentityAccess, UserAccessEntraProvider>
{}
