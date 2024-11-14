using DevDad.SaaSAdmin.RulesAccess.Abstractions;
using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.RulesAccess.AzureTableProvider
{
	internal class AzureTableProvider : IRulesAccess
	{
		private TableProviderOptions? _options;

        public AzureTableProvider()
        {
			
        }

		private string ConnectionString => _options?.ConnectionString
			?? throw new Exception($"AzureTableProvider component requires a ConnectionString.");

        public void LoadRules()
		{
			Console.WriteLine($"AzureTableProvider: LoadRules with {ConnectionString}");
		}

		public void SetConfiguration(IServiceOptions options)
		{
			_options = (TableProviderOptions)options;
		}

		public void StoreRule()
		{
			Console.WriteLine($"AzureTableProvider: StoreRule with {ConnectionString}");
		}

		
	}
}
