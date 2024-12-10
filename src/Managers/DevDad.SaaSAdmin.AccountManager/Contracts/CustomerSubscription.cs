using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevDad.SaaSAdmin.AccountManager.Contracts
{
	public class CustomerSubscription
	{
		public CustomerSubscription()
		{
			UserId = string.Empty;
			SKU = string.Empty;
			CurrentStatus = "None";
			History = new List<SubscriptionActivity>();
			Quotas = new UserQuotas();
		}

		public string UserId { get; set; }

		public string SKU { get; set; }

		public DateTime StartDateUtc { get; set; }

		public DateTime EndDateUtc { get; set; }

		public bool WillRenew { get; set; }

		public string CurrentStatus { get; set; }

		public UserQuotas Quotas { get; set; }

		public List<SubscriptionActivity> History { get; set; } = new List<SubscriptionActivity>();
	}
}
