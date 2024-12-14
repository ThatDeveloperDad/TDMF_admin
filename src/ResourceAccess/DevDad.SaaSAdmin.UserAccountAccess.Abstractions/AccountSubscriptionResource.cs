using System;
using ThatDeveloperDad.iFX.DomainUtilities;
using ThatDeveloperDad.iFX.DomainUtilities.Attributes;
using ThatDeveloperDad.iFX.ServiceModel.Taxonomy;
using Entities = DevDad.SaaSAdmin.iFX.DomainConstants.SaaSDomain;

namespace DevDad.SaaSAdmin.UserAccountAccess.Abstractions;

[DomainEntity(
    entityName: Entities.ApplicationSubscription.EntityName,
    declaringArchetype: ComponentArchetype.Manager)]
public class AccountSubscriptionResource: IdiomaticType
{
    [EntityAttribute(Entities.ApplicationSubscription.Attributes.UserId)]
    public string UserId { get; set; } = string.Empty;
    
    [EntityAttribute(Entities.ApplicationSubscription.Attributes.SKU)]
    public string SKU { get; set; } = string.Empty;

    [EntityAttribute(Entities.ApplicationSubscription.Attributes.CurrentPeriodStartUtc)]
    public DateTime StartDateUtc { get; set; }

    [EntityAttribute(Entities.ApplicationSubscription.Attributes.CurrentPeriodEndUtc)]
    public DateTime EndDateUtc { get; set; }

    [EntityAttribute(Entities.ApplicationSubscription.Attributes.WillRenew)]
    public bool WillRenew { get; set; }

    [EntityAttribute(Entities.ApplicationSubscription.Attributes.CurrentStatus)]
    public string CurrentStatus { get; set; } = "None";

    [EntityAttribute(
        entityAttributeName: Entities.ApplicationSubscription.Attributes.Quotas,
        isCollection: false,
        valueEntityName: Entities.UserQuotas.EntityName)] 
    public UserQuotaResource? Quotas { get; set; }

     [EntityAttribute(
        entityAttributeName: Entities.ApplicationSubscription.Attributes.SubscriptionHistory,
        isCollection: true,
        valueEntityName: Entities.SubscriptionHistoryEntry.EntityName)]
    public List<SubscriptionActivityResource> History { get; set; } = new List<SubscriptionActivityResource>();
}
