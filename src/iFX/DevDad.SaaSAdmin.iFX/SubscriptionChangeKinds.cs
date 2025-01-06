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
            { ActivityKind_Expire, SubscriptionStatuses.Expiring },
            { ActivityKind_Cancel, SubscriptionStatuses.Cancelled },
            { ActivityKind_Suspend, SubscriptionStatuses.Suspended },
            { ActivityKind_Resume, SubscriptionStatuses.Active }
        };

    /// <summary>
    /// Adds a new subscription to the Customer's account,
    /// if valid for current Susbcription Type and Status.
    /// A Free subscription is the Default state, and can
    /// always be overwritten with a Paid Subscription.
    /// 
    /// A Paid Subscription can only be overwritten with a
    /// free subscription if it is in a Cancelled State.
    /// </summary>
    public const string ActivityKind_Create = "Create";
    
    /// <summary>
    /// Extends the subscription by the Renewal Period.
    /// WillRenew = True.
    /// Status = Active.
    /// EndDate = EndDate + RenewalPeriod.
    /// Quotas:
    ///   - Refillable Quotas have Consumption set to Zero.
    ///   - Static Quotas are not modified.
    /// </summary>
    public const string ActivityKind_Renew = "Renew";
    
    /// <summary>
    /// Allows the subscirption to end on the last paid day.
    /// WillRenew = False.
    /// Status = Expiring.
    /// </summary>
    public const string ActivityKind_Expire = "Expire";
    
    /// <summary>
    /// Cancels the subscription immediately. 
    /// EndDate = Now.
    /// WillRenew = False.
    /// Status = Cancelled.
    /// Quotas = Budget is set to Zero.
    /// </summary>
    public const string ActivityKind_Cancel = "Cancel";
    
    /// <summary>
    /// Suspends the subscription immediately.
    /// WillRenew = False.
    /// Status = Suspended.
    /// Quotas = Set Budget to Consumption for all.
    /// </summary>
    public const string ActivityKind_Suspend = "Suspend";
    
    /// <summary>
    /// Resumes a suspended subscription.
    /// WillRenew = True
    /// Status = Active
    /// Quotas = Set Budget to Templated amounts.
    /// EndDate = Calc suspended Days, add to Now.
    /// </summary>
    public const string ActivityKind_Resume = "Resume";

}
