using System;
using ThatDeveloperDad.iFX.DomainUtilities;
using ThatDeveloperDad.iFX.DomainUtilities.Attributes;
using ThatDeveloperDad.iFX.ServiceModel.Taxonomy;
using Entities = DevDad.SaaSAdmin.iFX.DomainConstants.SaaSDomain;

namespace DevDad.SaaSAdmin.UserAccountAccess.Abstractions;

[DomainEntity(
    entityName: Entities.ApplicationUser.EntityName,
    declaringArchetype: ComponentArchetype.ResourceAccess)]
public class UserAccountResource : IdiomaticType
{
    [EntityAttribute(Entities.ApplicationUser.Attributes.UserId)]
    public string UserId { get; set; } = string.Empty;

    [EntityAttribute(Entities.ApplicationUser.Attributes.DisplayName)]
    public string DisplayName { get; set; } = string.Empty;

    [EntityAttribute(Entities.ApplicationUser.Attributes.SubscriptionStatus)]
    public string SubscriptionStatus { get; set; } = "None";

    [EntityAttribute(
        entityAttributeName:Entities.ApplicationUser.Attributes.CorrelationIds,
        isCollection:true,
        valueEntityName:Entities.ExternalUserId.EntityName)]
    public List<UserIdResource> ExternalIds { get; set; } = new List<UserIdResource>();

    [EntityAttribute(
        entityAttributeName:Entities.ApplicationUser.Attributes.Subscription,
        valueEntityName:Entities.ApplicationSubscription.EntityName,
        isCollection:false)]
    public AccountSubscriptionResource? Subscription { get; set; }
}
