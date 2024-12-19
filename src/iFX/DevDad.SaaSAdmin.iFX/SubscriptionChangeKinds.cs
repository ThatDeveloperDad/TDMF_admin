using System;

namespace DevDad.SaaSAdmin.iFX;

public class SubscriptionChangeKinds
{

    public static readonly string[] AllowedValues 
        = {
            ActivityKind_Create,
            ActivityKind_Renew,
            ActivityKind_Expire,
            ActivityKind_Cancel,
            ActivityKind_Suspend,
            ActivityKind_Resume
        };

    public static readonly Dictionary<string, string> ActivityStatusResult 
        = new Dictionary<string, string>
        {
            { ActivityKind_Create, SubscriptionStatuses.Active },
            { ActivityKind_Renew, SubscriptionStatuses.Active },
            { ActivityKind_Expire, SubscriptionStatuses.Expired },
            { ActivityKind_Cancel, SubscriptionStatuses.Cancelled },
            { ActivityKind_Suspend, SubscriptionStatuses.Suspended },
            { ActivityKind_Resume, SubscriptionStatuses.Active }
        };

    public const string ActivityKind_Create = "Create";
    public const string ActivityKind_Renew = "Renew";
    public const string ActivityKind_Expire = "Expire";
    public const string ActivityKind_Cancel = "Cancel";
    public const string ActivityKind_Suspend = "Suspend";
    public const string ActivityKind_Resume = "Resume";

}
