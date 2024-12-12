using ThatDeveloperDad.iFX.CollectionUtilities;
using ThatDeveloperDad.iFX.ServiceModel.Taxonomy;

namespace DevDad.SaaSAdmin.Catalog.Abstractions;

public interface ICatalogAccess:IResourceAccessService
{
    Task<SubscriptionTemplateResource?> GetCatalogItemAsync(string sku);
    Task<IEnumerable<FilteredSubscriptionItem>> FilterCatalogAsync(Filter<SubscriptionTemplateResource> filter);
}
