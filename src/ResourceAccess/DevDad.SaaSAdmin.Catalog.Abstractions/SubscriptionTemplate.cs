using System;
using System.Reflection;
using ThatDeveloperDad.iFX.DomainUtilities;
using ThatDeveloperDad.iFX.DomainUtilities.Attributes;
using ThatDeveloperDad.iFX.ServiceModel.Taxonomy;

namespace DevDad.SaaSAdmin.Catalog.Abstractions;

public class SubscriptionTemplateResource 
{
    public SubscriptionTemplateResource()
    {
        SKU = string.Empty;
        Name = string.Empty;
        Description = string.Empty;
        ConfersMembershipIn = Array.Empty<string>();
        RenewalPeriodDays = 0;
        ResourceGrants = new List<QuotaGrantResource>();
        AllowedStatuses = Array.Empty<string>();
        DefaultStatus = string.Empty;
    }

    /// <summary>
    /// SKU stands for stock keeping unit.
    /// We'll use this as a replacement for "ID"
    /// </summary>
    public string SKU { get; set; }

    /// <summary>
    /// Represents the ItemID used in setting up a purchase of the subscription
    /// within the ECommerce Vendor's systems.
    /// 
    /// With LemonSqueezy, this valur corresponds to the required variant_id
    /// in the CreateCheckout call.
    /// </summary>
    public string? VendorItemId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    /// <summary>
    /// This is an array of Groups that anyone with this SKU will receive membership in.
    /// </summary>
    public string[] ConfersMembershipIn { get; set; }

    /// <summary>
    /// Describes how often the subscription will renew.
    public int RenewalPeriodDays { get; set; }

    public List<QuotaGrantResource> ResourceGrants { get; private set; }

    /// <summary>
    /// Provides the lists of Statuses that are allowed for this Subsctiption Template.
    /// </summary>
    public string[] AllowedStatuses { get; set; }

    public string DefaultStatus { get; set; }
}
