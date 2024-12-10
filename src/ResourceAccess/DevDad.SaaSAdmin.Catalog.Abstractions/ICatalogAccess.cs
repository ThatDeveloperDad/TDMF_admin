using ThatDeveloperDad.iFX.ServiceModel.Taxonomy;

namespace DevDad.SaaSAdmin.Catalog.Abstractions;

public interface ICatalogAccess:IResourceAccessService
{
    Task<SubscriptionTemplate?> GetCatalogItemAsync(string sku);
    Task<IEnumerable<FilteredSubscriptionItem>> FilterCatalog(string[] filter);
}
