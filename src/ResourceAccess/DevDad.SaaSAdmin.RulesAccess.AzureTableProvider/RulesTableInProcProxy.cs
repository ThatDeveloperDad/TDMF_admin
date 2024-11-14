using DevDad.SaaSAdmin.RulesAccess.Abstractions;
using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.RulesAccess.AzureTableProvider
{
	internal class RulesTableInProcProxy
		: InProcServiceProxy<IRulesAccess>
		  , IRulesAccess
	{
		public RulesTableInProcProxy(IRulesAccess implementation) : base(implementation)
		{
		}

		public void LoadRules()
		{
			_service.LoadRules();
		}

		//This might have to go.
		public void SetConfiguration(IServiceOptions options)
		{
			throw new NotImplementedException();
		}

		public void StoreRule()
		{
			_service.StoreRule();
		}
	}
}
