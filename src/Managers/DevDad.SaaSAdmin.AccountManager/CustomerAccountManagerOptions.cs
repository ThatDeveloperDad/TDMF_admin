﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.AccountManager
{
	public class CustomerAccountManagerOptions : IServiceOptions
	{
		public const string ConfigSectionName = "CustomerAccountManager";

        public CustomerAccountManagerOptions()
        {
            
        }		
	}
}
