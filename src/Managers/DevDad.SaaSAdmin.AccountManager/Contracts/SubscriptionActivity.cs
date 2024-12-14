using System;
using ThatDeveloperDad.iFX.DomainUtilities;
using ThatDeveloperDad.iFX.DomainUtilities.Attributes;
using ThatDeveloperDad.iFX.ServiceModel.Taxonomy;
using Entities = DevDad.SaaSAdmin.iFX.DomainConstants.SaaSDomain;

namespace DevDad.SaaSAdmin.AccountManager.Contracts;

/// <summary>
/// Describes an activity that altered a Customer's Subscription in 
/// some way.  This is purely for reporting purposes.
/// </summary>
[DomainEntity(
    entityName: Entities.SubscriptionHistoryEntry.EntityName, 
    declaringArchetype: ComponentArchetype.Manager)]
public class SubscriptionActivity:IdiomaticType
{

    [EntityAttribute(Entities.SubscriptionHistoryEntry.Attributes.ActivityKind)]
    public string ActivityKind { get; set; } = string.Empty;

    [EntityAttribute(Entities.SubscriptionHistoryEntry.Attributes.ActivityDateUtc)]
    public DateTime ActivityDateUTC { get; set; } = DateTime.UtcNow;
}
