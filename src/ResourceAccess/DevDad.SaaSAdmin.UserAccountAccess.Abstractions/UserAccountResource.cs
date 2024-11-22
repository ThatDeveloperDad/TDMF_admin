using System;

namespace DevDad.SaaSAdmin.UserAccountAccess.Abstractions;

public class UserAccountResource
{
    public string UserId { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string SubscriptionStatus { get; set; } = "None";

    public List<UserIdResource> ExternalIds { get; set; } = new List<UserIdResource>();

    public AccountSubscriptionResource? Subscription { get; set; }
}
