using DevDad.SaaSAdmin.Catalog.Abstractions;
using DevDad.SaaSAdmin.iFX;

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

    private List<SubscriptionTemplate> _catalog;

    public StaticCatalogProvider()
    {
        _catalog = PopulateCatalog();
    }

    public Task<IEnumerable<FilteredSubscriptionItem>> FilterCatalog(string[] filter)
    {
        List<FilteredSubscriptionItem> filtered = new();

        //TODO:  Create generic Filter capability and integrate it here.
        filtered = _catalog.Select(ci=>
            new FilteredSubscriptionItem(){
                SKU = ci.SKU,
                Name = ci.Name,
                Description = ci.Description
            }
        )
        .ToList();

        return Task.FromResult(filtered.AsEnumerable());
    }

    public Task<SubscriptionTemplate?> GetCatalogItemAsync(string sku)
    {
        SubscriptionTemplate? template = null;

        template = _catalog.FirstOrDefault(x=> x.SKU == sku);

        return Task.FromResult(template);
    }

    private List<SubscriptionTemplate> PopulateCatalog()
    {
        List<SubscriptionTemplate> catalog = new();

        SubscriptionTemplate freeTemplate = new(){
            SKU = "DM-FAMILIAR-FREE",
            Name = "The DM's Familiar - Free Tier",
            Description = "Provides access to the Random NPC Generator, and any characters created and saved during a paid subscription time.",
            RenewalPeriod = SubscriptionRenewalFrequencies.Permanent,
            ConfersMembershipIn = new string[]{EntraGroups.FreeUsers},
            AllowedStatuses = new string[]{SubscriptionStatuses.Active},
            DefaultStatus = SubscriptionStatuses.Active
        };
        freeTemplate.ResourceGrants.AddRange(GetFreeSubscriptionGrants());
        catalog.Add(freeTemplate);

        SubscriptionTemplate paidTemplate = new(){
            SKU = "DM-FAMILIAR-PD-MONTHLY",
            Name = "The DM's Familiar - Monthly Subscription",
            Description = "Provides access to the RandomNPC Generator, up to 100 AI Descriptions per month, and Storage Space for up to 100 NPCs.",
            RenewalPeriod = SubscriptionRenewalFrequencies.Monthly,
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

    private List<ResourceGrant> GetFreeSubscriptionGrants()
    {
        List<ResourceGrant> grants = new();

        grants.Add(new ResourceGrant(){
            ResourceKind = MeteredResourceKinds.NpcStorage,
            InitialBudget = 5,
            RenewalBudget = 0
        });

        grants.Add(new ResourceGrant(){
            ResourceKind = MeteredResourceKinds.NpcAiDetail,
            InitialBudget = 0,
            RenewalBudget = 0
        });

        return grants;
    }

    private List<ResourceGrant> GetPaidSubscriptionGrants()
    {
        List<ResourceGrant> grants = new();

        grants.Add(new ResourceGrant(){
            ResourceKind = MeteredResourceKinds.NpcStorage,
            InitialBudget = 100,
            RenewalBudget = 0
        });

        grants.Add(new ResourceGrant(){
            ResourceKind = MeteredResourceKinds.NpcAiDetail,
            InitialBudget = 100,
            RenewalBudget = 100
        });

        return grants;
    }
}
