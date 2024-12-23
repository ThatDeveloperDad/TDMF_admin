using System;
using System.Reflection.Metadata;

namespace DevDad.SaaSAdmin.iFX;

/// <summary>
/// Describes, for each Subscription State, which Activities are valid.
/// Similarly, for each Activity Kind, describes the resulting Subscription Status that will
/// be assigned by the change.
/// </summary>
public class SubscriptionStateTransitions
{
    /// <summary>
    /// Describes which Statuses a Subscription can transition TO from its current status.
    /// </summary>
    public static readonly Dictionary<string, string[]> ValidTargetStatuses = new Dictionary<string, string[]>()
    {
        // Note:  we might not need this.  ValidStatusActions might give us what we 
        // intended to set up, but better.
        { string.Empty, // When NO current subscription is present on the account.
            new string[] 
            {
                SubscriptionStatuses.Pending,
                SubscriptionStatuses.Active
            }
        },
        { SubscriptionStatuses.Active,
            new string[] 
            {
                SubscriptionStatuses.Cancelled,
                SubscriptionStatuses.Suspended,
                SubscriptionStatuses.Expiring
            }
        },
        {
            SubscriptionStatuses.Expiring,
            new string[]
            {
                SubscriptionStatuses.Pending,
                SubscriptionStatuses.Active,
                SubscriptionStatuses.Cancelled,
                SubscriptionStatuses.Expired
            }
        },
        {
            SubscriptionStatuses.Expired,
            new string[]
            {
                SubscriptionStatuses.Pending,
                SubscriptionStatuses.Active,
                SubscriptionStatuses.Cancelled
            }
        },
        { SubscriptionStatuses.Cancelled,
            new string[] {}
        },
        { SubscriptionStatuses.Suspended,
            new string[] 
            {
                SubscriptionStatuses.Active
            }
        },
    }; 

    /// <summary>
    /// Describes which SubscriptionChangeKinds can be performed on a subscription
    /// based on the existing subscription's status.
    /// </summary>
    public static readonly Dictionary<string, string[]> ValidStatusActions = new Dictionary<string, string[]>
    {
        { 
            string.Empty, // When there isn't a current subscription
            new string[]
            {
                SubscriptionChangeKinds.ActivityKind_Create
            }
        },
        {
            SubscriptionStatuses.Active,
            new string[]
            {
                SubscriptionChangeKinds.ActivityKind_Renew,
                SubscriptionChangeKinds.ActivityKind_Suspend,
                SubscriptionChangeKinds.ActivityKind_Expire,
                SubscriptionChangeKinds.ActivityKind_Cancel
            }
        },
        {
            SubscriptionStatuses.Expired,
            new string[]
            {
                SubscriptionChangeKinds.ActivityKind_Renew,
                SubscriptionChangeKinds.ActivityKind_Cancel
            }
        },
        {
            SubscriptionStatuses.Expiring,
            new string[]
            {
                SubscriptionChangeKinds.ActivityKind_Renew,
                SubscriptionChangeKinds.ActivityKind_Cancel
            }
        },
        {
            SubscriptionStatuses.Suspended,
            new string[]
            {
                SubscriptionChangeKinds.ActivityKind_Resume,
                SubscriptionChangeKinds.ActivityKind_Cancel
            }
        },
        {
            SubscriptionStatuses.Cancelled,
            new string[]
            {
                SubscriptionChangeKinds.ActivityKind_Create
            }
        }
    };

    /// <summary>
    /// Used to determin if a given activity can be performed on a subscription, given
    /// the subscription's current state.
    /// 
    /// These values are mapped in the ValidStatusActions dictionary.
    /// </summary>
    /// <param name="subscriptionStatus"></param>
    /// <param name="actionName"></param>
    /// <returns></returns>
    public static bool CanPerformActivityForStatus(string actionName, string subscriptionStatus)
    {
        bool activityIsValid = ValidStatusActions[subscriptionStatus].Contains(actionName);
        
        var statusHasActions = ValidStatusActions.TryGetValue(subscriptionStatus, out string[]? validActions);
        if(statusHasActions == false)
        {
            return false;
        }

        if(validActions?.Contains(actionName) == false)
        {
            return false;
        }

        return activityIsValid;
    }
}
