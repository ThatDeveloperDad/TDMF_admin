using DevDad.SaaSAdmin.Catalog.Abstractions;
using DevDad.SaaSAdmin.iFX;
using ThatDeveloperDad.iFX.CollectionUtilities;

namespace DevDad.SaaSAdmin.Catalog.HardCoded;

/// <summary>
/// This class provides the Subscription & Product Catalog as manually built, 
/// hardcoded Subscription Template definitions.
/// 
/// I have only a couple SubscriptionKinds at the moment, and they're not subject to
/// very much change as of today.  So, I'm just going to hardcode them here.
/// </summary>
public class StaticCatalogProvider : ICatalogAccess
{
    public const string SKUS_TDMF_PD_MNTH = "DM-FAMILIAR-PD-MONTHLY";
    public const string SKUS_TDMF_FR = "DM-FAMILIAR-FREE";
    
    private List<SubscriptionTemplateResource> _catalog;

    public StaticCatalogProvider()
    {
        _catalog = PopulateCatalog();
    }

    public static IEnumerable<SubscriptionTemplateResource> GetCatalog()
    {
        var cp = new StaticCatalogProvider();
        return cp.PopulateCatalog();
    }

    public Task<IEnumerable<FilteredSubscriptionItem>> FilterCatalogAsync(
        Filter<SubscriptionTemplateResource> filter)
    {
        List<FilteredSubscriptionItem> filtered = new();

        var filteredCatalog = filter.ApplyFilter(_catalog);

        //Now, convert the CatalogItems into the FilteredSubscriptionItems.
        filtered.AddRange(filteredCatalog
            .Select(fc=> new FilteredSubscriptionItem(){
                SKU = fc.SKU,
                Name = fc.Name,
                Description = fc.Description
            }));

        return Task.FromResult(filtered.AsEnumerable());
    }

    public Task<SubscriptionTemplateResource?> GetCatalogItemAsync(string sku)
    {
        SubscriptionTemplateResource? template = null;

        template = _catalog.FirstOrDefault(x=> x.SKU == sku);

        return Task.FromResult(template);
    }

    private List<SubscriptionTemplateResource> PopulateCatalog()
    {
        List<SubscriptionTemplateResource> catalog = new();

        SubscriptionTemplateResource freeTemplate = new(){
            SKU = SKUS_TDMF_FR,
            Name = "The DM's Familiar - Free Tier",
            Description = "Provides access to the Random NPC Generator, and any characters created and saved during a paid subscription time.",
            RenewalPeriodDays = SubscriptionRenewalFrequencies.Permanent,
            ConfersMembershipIn = new string[]{EntraGroups.FreeUsers},
            AllowedStatuses = new string[]{SubscriptionStatuses.Active},
            DefaultStatus = SubscriptionStatuses.Active
        };
        freeTemplate.ResourceGrants.AddRange(GetFreeSubscriptionGrants());
        catalog.Add(freeTemplate);

        SubscriptionTemplateResource paidTemplate = new(){
            SKU = SKUS_TDMF_PD_MNTH,
            Name = "The DM's Familiar - Monthly Subscription",
            Description = "Provides access to the RandomNPC Generator, up to 100 AI Descriptions per month, and Storage Space for up to 100 NPCs.",
            RenewalPeriodDays = SubscriptionRenewalFrequencies.Monthly,
            ConfersMembershipIn = new string[]
                {
                    EntraGroups.FreeUsers, 
                    EntraGroups.PaidUsers
                },
            AllowedStatuses = new string[]
                {
                    SubscriptionStatuses.Active,
                    SubscriptionStatuses.Expiring,
                    SubscriptionStatuses.Expired,
                    SubscriptionStatuses.Cancelled,
                    SubscriptionStatuses.Pending
                },
            DefaultStatus = SubscriptionStatuses.Pending
        };
        paidTemplate.ResourceGrants.AddRange(GetPaidSubscriptionGrants());
        catalog.Add(paidTemplate);

        return catalog;
    }

    private List<QuotaGrantResource> GetFreeSubscriptionGrants()
    {
        List<QuotaGrantResource> grants = new();

        grants.Add(new QuotaGrantResource(){
            ResourceKind = MeteredResourceKinds.NpcStorage,
            InitialBudget = 5,
            RenewalBudget = 0
        });

        grants.Add(new QuotaGrantResource(){
            ResourceKind = MeteredResourceKinds.NpcAiDetail,
            InitialBudget = 0,
            RenewalBudget = 0
        });

        return grants;
    }

    private List<QuotaGrantResource> GetPaidSubscriptionGrants()
    {
        List<QuotaGrantResource> grants = new();

        grants.Add(new QuotaGrantResource(){
            ResourceKind = MeteredResourceKinds.NpcStorage,
            InitialBudget = 100,
            RenewalBudget = 0
        });

        grants.Add(new QuotaGrantResource(){
            ResourceKind = MeteredResourceKinds.NpcAiDetail,
            InitialBudget = 100,
            RenewalBudget = 100
        });

        return grants;
    }
}
