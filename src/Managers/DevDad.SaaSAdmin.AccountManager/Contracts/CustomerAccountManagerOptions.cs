using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.AccountManager.Contracts
{
	public class CustomerAccountManagerOptions : IServiceOptions
	{
		public const string ConfigSectionName = "CustomerAccountManager";

        public CustomerAccountManagerOptions()
        {
            
        }

        public string SomeContrivedNonsense { get; set; } = "Booga booga!";

		
	}
}
