using System;
using ThatDeveloperDad.iFX.DomainUtilities;
using ThatDeveloperDad.iFX.DomainUtilities.Attributes;
using ThatDeveloperDad.iFX.ServiceModel.Taxonomy;
using Entities = DevDad.SaaSAdmin.iFX.DomainConstants.SaaSDomain;

namespace DevDad.SaaSAdmin.AccountManager.Contracts;

[DomainEntity(
    entityName: Entities.ExternalUserId.EntityName,
    declaringArchetype: ComponentArchetype.Manager)]
public class ExternalId : IdiomaticType
{
    [EntityAttribute(Entities.ExternalUserId.Attributes.RemoteSystemName)]
    public string Vendor { get; set; } = string.Empty;
    
    [EntityAttribute(Entities.ExternalUserId.Attributes.IdAtRemote)]
    public string IdAtVendor { get; set; } = string.Empty;
}
