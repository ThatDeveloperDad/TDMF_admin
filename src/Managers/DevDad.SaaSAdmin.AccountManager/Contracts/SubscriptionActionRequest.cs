using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevDad.SaaSAdmin.AccountManager.Contracts
{
	public class SubscriptionActionRequest
	{

		public SubscriptionActionRequest()
		{
			ActivityName = string.Empty;
			RequestSource = string.Empty;
			CustomerId = string.Empty;
			CustomerIdSource = string.Empty;
			SubscriptionSku = string.Empty;
		}

		/// <summary>
		/// Identifies what activity we need to perform on the customer's subscription.
		/// </summary>
		public string ActivityName { get; set; }

		/// <summary>
		/// The name of the system from which the Activity originated.
		/// </summary>
		public string RequestSource { get; set; }

		/// <summary>
		/// The identifier for the customer whose subscription we are altering.
		/// </summary>
		public string CustomerId { get; set; }

		/// <summary>
		/// The vendor who "owns" the provided CustomerId.
		/// </summary>
		public string CustomerIdSource { get; set; }

		/// <summary>
		/// Identifies which subscription the request is for.
		/// </summary>
		public string SubscriptionSku { get; set; }
	}
}
