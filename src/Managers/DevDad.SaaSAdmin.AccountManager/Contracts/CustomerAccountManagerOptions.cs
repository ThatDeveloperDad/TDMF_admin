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

		public static CustomerAccountManagerOptions FromConfiguration(IConfigurationSection config)
		{
			//var mysection = config.GetSection(ConfigSectionName);

			//if(mysection == null)
			//{
			//	throw new Exception($"Missing configuration section {ConfigSectionName}");
			//}

			CustomerAccountManagerOptions options = new();
			config.Bind(options);

			return options;

		}

        private CustomerAccountManagerOptions()
        {
            
        }

        public string SomeContrivedNonsense { get; set; } = "Booga booga!";

		
	}
}
