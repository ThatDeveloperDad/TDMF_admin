using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThatDeveloperDad.iFX.DomainUtilities;
using ThatDeveloperDad.iFX.DomainUtilities.Attributes;
using ThatDeveloperDad.iFX.ServiceModel.Taxonomy;
using Entities = DevDad.SaaSAdmin.iFX.DomainConstants.SaaSDomain;

namespace DevDad.SaaSAdmin.AccountManager.Contracts
{
	[DomainEntity(
		entityName:Entities.ApplicationUser.EntityName,
		declaringArchetype:ComponentArchetype.Manager)]
	public class CustomerProfile :IdiomaticType
	{
		[EntityAttribute(Entities.ApplicationUser.Attributes.UserId)]
		public string UserId { get; set; } = string.Empty;
		
		[EntityAttribute(Entities.ApplicationUser.Attributes.DisplayName)]
		public string DisplayName { get; set; } = string.Empty;

		public CustomerSubscription? Subscription { get; set; }

		public string SubscriptionStatus { get; set; } = string.Empty;

		public List<ExternalId> ExternalIds { get; set; } = new();
	}
}
