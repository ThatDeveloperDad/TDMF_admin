using System;
using ThatDeveloperDad.iFX.DomainUtilities;
using ThatDeveloperDad.iFX.DomainUtilities.Attributes;
using ThatDeveloperDad.iFX.ServiceModel.Taxonomy;
using Entities = DevDad.SaaSAdmin.iFX.DomainConstants.SaaSDomain;

namespace DevDad.SaaSAdmin.UserAccountAccess.Abstractions;

[DomainEntity(
    entityName: Entities.SubscriptionHistoryEntry.EntityName, 
    declaringArchetype: ComponentArchetype.Manager)]
public class SubscriptionActivityResource:IdiomaticType
{
        [EntityAttribute(Entities.SubscriptionHistoryEntry.Attributes.ActivityKind)]
        public string ActivityKind { get; set; } = string.Empty;

        [EntityAttribute(Entities.SubscriptionHistoryEntry.Attributes.ActivityDateUtc)]
        public DateTime ActivityDateUTC { get; set; } = DateTime.UtcNow;

        [EntityAttribute(Entities.SubscriptionHistoryEntry.Attributes.Comment)]
        public string Comment { get; set; } = string.Empty;
}
