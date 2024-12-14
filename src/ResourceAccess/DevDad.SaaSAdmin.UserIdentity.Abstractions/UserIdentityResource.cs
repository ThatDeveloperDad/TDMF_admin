using System;
using ThatDeveloperDad.iFX.DomainUtilities;
using ThatDeveloperDad.iFX.DomainUtilities.Attributes;
using ThatDeveloperDad.iFX.ServiceModel.Taxonomy;
using Entities = DevDad.SaaSAdmin.iFX.DomainConstants.SaaSDomain;

namespace DevDad.SaaSAdmin.UserIdentity.Abstractions;

[DomainEntity(
    entityName: Entities.ApplicationUser.EntityName,
    declaringArchetype: ComponentArchetype.ResourceAccess)]
public class UserIdentityResource : IdiomaticType
{
    public UserIdentityResource()
    {
        UserId = string.Empty;
        DisplayName = string.Empty;
    }

    public UserIdentityResource(string userId, string displayName)
    {
        UserId = userId;
        DisplayName = displayName;
    }

    [EntityAttribute(Entities.ApplicationUser.Attributes.UserId)]
    public string UserId { get; init; }
    
    [EntityAttribute(Entities.ApplicationUser.Attributes.DisplayName)]
    public string DisplayName { get; init; }
}
