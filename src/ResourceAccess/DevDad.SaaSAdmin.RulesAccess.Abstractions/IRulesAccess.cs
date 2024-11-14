using ThatDeveloperDad.iFX.ServiceModel.Taxonomy;

namespace DevDad.SaaSAdmin.RulesAccess.Abstractions
{
	//Temporary Interface while I experiment.
	public interface IRulesAccess : IResourceAccessService
	{
		void LoadRules();
		void StoreRule();
	}
}
