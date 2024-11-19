using DevDad.SaaSAdmin.RulesAccess.Abstractions;
using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.RulesAccess.AzureTableProvider
{
	internal class RulesTableProxyBuilder
		: DynamicServiceProxy<IRulesAccess, AzureTableProvider>
	{}
}
