using System;

namespace DevDad.SaaSAdmin.AccountManager.Contracts;

/// <summary>
/// Describes an activity that altered a Customer's Subscription in 
/// some way.  This is purely for reporting purposes.
/// </summary>
public class SubscriptionActivity
{
    public const string ActivityKind_Created = "Created";
    public const string ActivityKind_Updated = "Renewed";
    public const string ActivityKind_Expired = "Expired";
    public const string ActivityKind_Cancelled = "Cancelled";
    public const string ActivityKind_Suspended = "Suspended";
    public const string ActivityKind_Resumed = "Resumed";

    public string ActivityKind { get; set; } = string.Empty;

    public DateTime ActivityDateUTC { get; set; } = DateTime.UtcNow;
}
