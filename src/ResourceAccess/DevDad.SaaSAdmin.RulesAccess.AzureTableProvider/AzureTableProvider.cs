using DevDad.SaaSAdmin.RulesAccess.Abstractions;
using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.RulesAccess.AzureTableProvider
{
	internal class AzureTableProvider 
	: IRulesAccess
	{
		public AzureTableProvider()
        {
			ConnectionString = string.Empty;
        }

		private string ConnectionString {get;set;}

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
