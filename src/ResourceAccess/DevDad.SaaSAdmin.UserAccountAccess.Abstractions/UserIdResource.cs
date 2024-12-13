using System;
using ThatDeveloperDad.iFX.DomainUtilities;
using ThatDeveloperDad.iFX.DomainUtilities.Attributes;
using ThatDeveloperDad.iFX.ServiceModel.Taxonomy;
using Entities = DevDad.SaaSAdmin.iFX.DomainConstants.SaaSDomain;


namespace DevDad.SaaSAdmin.UserAccountAccess.Abstractions;

[DomainEntity(
    entityName: Entities.ExternalUserId.EntityName,
    declaringArchetype: ComponentArchetype.ResourceAccess)]
public class UserIdResource:IdiomaticType
{
    [EntityAttribute(Entities.ExternalUserId.Attributes.RemoteSystemName)]
    public string VendorName { get; set; } = string.Empty;
    [EntityAttribute(Entities.ExternalUserId.Attributes.IdAtRemote)]
    public string UserIdAtVendor { get; set; } = string.Empty;
}
