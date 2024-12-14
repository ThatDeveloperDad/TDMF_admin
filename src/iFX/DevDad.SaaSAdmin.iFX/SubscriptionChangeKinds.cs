using System;

namespace DevDad.SaaSAdmin.iFX;

public class SubscriptionChangeKinds
{

    public readonly string[] AllowedValues = 
    {
        ActivityKind_Created,
        ActivityKind_Updated,
        ActivityKind_Expired,
        ActivityKind_Cancelled,
        ActivityKind_Suspended,
        ActivityKind_Resumed
    };

    public const string ActivityKind_Created = "Created";
    public const string ActivityKind_Updated = "Renewed";
    public const string ActivityKind_Expired = "Expired";
    public const string ActivityKind_Cancelled = "Cancelled";
    public const string ActivityKind_Suspended = "Suspended";
    public const string ActivityKind_Resumed = "Resumed";

}
