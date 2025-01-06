using System.Collections.Generic;
using System.Linq;
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

 		[EntityAttribute(
			entityAttributeName:Entities.ApplicationUser.Attributes.Subscription,
			valueEntityName:Entities.ApplicationSubscription.EntityName,
			isCollection:false)] 
		public CustomerSubscription? Subscription { get; set; }

		[EntityAttribute(Entities.ApplicationUser.Attributes.SubscriptionStatus)]
		public string SubscriptionStatus { get; set; } = string.Empty;

		[EntityAttribute(
			entityAttributeName:Entities.ApplicationUser.Attributes.CorrelationIds,
			isCollection:true,
			valueEntityName:Entities.ExternalUserId.EntityName)]
		public List<ExternalId> ExternalIds { get; set; } = new();

		internal string? GetUserIdForVendor(string vendorName)
		{
			string? id = null;

			id = ExternalIds
				.FirstOrDefault(e=> e.Vendor == vendorName)?
				.IdAtVendor;

			return id;
		}
	}
}
