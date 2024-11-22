using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.RulesAccess.AzureTableProvider
{
	public class TableProviderOptions : IServiceOptions
	{
		public string ConnectionString { get; set; } = string.Empty;
    }
}
