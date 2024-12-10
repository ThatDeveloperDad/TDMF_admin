using System;

namespace DevDad.SaaSAdmin.Catalog.Abstractions;

public class FilteredSubscriptionItem
{

    public FilteredSubscriptionItem()
    {
        SKU = string.Empty;
        Name = string.Empty;
        Description = string.Empty;
    }

    public string SKU { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

}
