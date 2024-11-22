using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevDad.SaaSAdmin.AccountManager.Contracts
{
	public class CustomerProfile
	{
		public string UserId { get; set; } = string.Empty;
		public string DisplayName { get; set; } = string.Empty;
		public string SubscriptionStatus { get; set; } = string.Empty;

		
	}
}
