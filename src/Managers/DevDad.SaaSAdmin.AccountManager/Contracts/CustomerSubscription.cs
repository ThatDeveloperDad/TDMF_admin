using System;
using System.Collections.Generic;
using ThatDeveloperDad.iFX.DomainUtilities;
using ThatDeveloperDad.iFX.DomainUtilities.Attributes;
using ThatDeveloperDad.iFX.ServiceModel.Taxonomy;
using Entities = DevDad.SaaSAdmin.iFX.DomainConstants.SaaSDomain;

namespace DevDad.SaaSAdmin.AccountManager.Contracts
{
	[DomainEntity(
		entityName: Entities.ApplicationSubscription.EntityName,
		declaringArchetype: ComponentArchetype.Manager)]
	public class CustomerSubscription : IdiomaticType
	{
		public CustomerSubscription()
		{
			UserId = string.Empty;
			SKU = string.Empty;
			CurrentStatus = "None";
			History = new List<SubscriptionActivity>();
			Quotas = new UserQuotas();
		}

		[EntityAttribute(Entities.ApplicationSubscription.Attributes.UserId)]
		public string UserId { get; set; }

		[EntityAttribute(Entities.ApplicationSubscription.Attributes.SKU)]
		public string SKU { get; set; }

		[EntityAttribute(Entities.ApplicationSubscription.Attributes.CurrentPeriodStartUtc)]
		public DateTime StartDateUtc { get; set; }

		[EntityAttribute(Entities.ApplicationSubscription.Attributes.CurrentPeriodEndUtc)]
		public DateTime EndDateUtc { get; set; }

		[EntityAttribute(Entities.ApplicationSubscription.Attributes.WillRenew)]
		public bool WillRenew { get; set; }

		[EntityAttribute(Entities.ApplicationSubscription.Attributes.CurrentStatus)]
		public string CurrentStatus { get; set; }

 		[EntityAttribute(
			entityAttributeName:Entities.ApplicationSubscription.Attributes.Quotas,
			isCollection:false,
			valueEntityName:Entities.UserQuotas.EntityName)]
		public UserQuotas Quotas { get; set; }

 		[EntityAttribute(
			entityAttributeName:Entities.ApplicationSubscription.Attributes.SubscriptionHistory,
			isCollection:true,
			valueEntityName:Entities.SubscriptionHistoryEntry.EntityName)] 
		public List<SubscriptionActivity> History { get; set; } = new List<SubscriptionActivity>();

		public void SetUserId(string userId)
		{
			UserId = userId;
			Quotas.UserId = userId;
		}
	}
}
