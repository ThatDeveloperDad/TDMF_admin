using System;

namespace DevDad.SaaSAdmin.UserAccountAccess.Abstractions;

public class AccountSubscriptionResource
{

    public string UserId { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;

    public DateTime StartDateUtc { get; set; }

    public DateTime EndDateUtc { get; set; }

    public bool WillRenew { get; set; }

    public string CurrentStatus { get; set; } = "None";

    public UserQuotaResource? Quotas { get; set; }

    public List<SubscriptionActivityResource> History { get; set; } = new List<SubscriptionActivityResource>();
}
