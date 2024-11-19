using DevDad.SaaSAdmin.RulesAccess.Abstractions;
using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.RulesAccess.AzureTableProvider
{
	internal class AzureTableProvider 
	: ServiceBase, IRulesAccess
	{
		public AzureTableProvider()
        {
			
        }

		private TableProviderOptions? Configuration
			=> Options<TableProviderOptions>();

		private string ConnectionString => Configuration?.ConnectionString
			?? throw new Exception($"AzureTableProvider component requires a ConnectionString.");

        public void LoadRules()
		{
			Console.WriteLine($"AzureTableProvider: LoadRules with {ConnectionString}");
		}
		
		public void StoreRule()
		{
			Console.WriteLine($"AzureTableProvider: StoreRule with {ConnectionString}");
		}

		
	}
}
