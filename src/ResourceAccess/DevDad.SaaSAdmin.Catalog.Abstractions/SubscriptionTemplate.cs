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
        RenewalPeriod = 0;
        ResourceGrants = new List<QuotaGrantResource>();
        AllowedStatuses = Array.Empty<string>();
        DefaultStatus = string.Empty;
    }

    /// <summary>
    /// SKU stands for stock keeping unit.
    /// We'll use this as a replacement for "ID"
    /// </summary>
    public string SKU { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    /// <summary>
    /// This is an array of Groups that anyone with this SKU will receive membership in.
    /// </summary>
    public string[] ConfersMembershipIn { get; set; }

    /// <summary>
    /// Describes how often the subscription will renew.
    public int RenewalPeriod { get; set; }

    public List<QuotaGrantResource> ResourceGrants { get; private set; }

    /// <summary>
    /// Provides the lists of Statuses that are allowed for this Subsctiption Template.
    /// </summary>
    public string[] AllowedStatuses { get; set; }

    public string DefaultStatus { get; set; }
}
