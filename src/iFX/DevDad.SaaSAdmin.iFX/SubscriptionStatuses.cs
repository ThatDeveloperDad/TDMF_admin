using System;

namespace DevDad.SaaSAdmin.iFX;

/// <summary>
/// Provides constants that name the different statuses a subscription
/// can be in.
/// 
/// Subscription Templates allow only a subset of these statuses.
/// and those subsets are defined in the SubscriptionTemplate provider.
/// </summary>
public class SubscriptionStatuses
{
    /// <summary>
    /// For Free and Paid subscriptions, the Active
    /// status means everything is "normal" and the
    /// user should have access to everything the 
    /// Template defines.
    /// </summary>
    public const string Active = "Active";

    /// <summary>
    /// Expiring is applied to a Paid subscription when
    /// the user has been billed, and places a request that
    /// we stop the automatic renewal of the subscription.
    /// </summary>
    public const string Expiring = "Expiring";

    /// <summary>
    /// Expired is applied to a Paid subscription when the
    /// customer has requested we cease automated billing,
    /// and the subscription's billing date has passed.
    /// 
    /// The user may access any data they'd saved during their paid time,
    /// but may not invoke any AI features, or save new objects.
    /// Existing objects may be edited or deleted once those
    /// features have been added to the Product.
    /// </summary>
    public const string Expired = "Expired";

    /// <summary>
    /// Cancelled is reserved for paid subscriptions where the user
    /// has been billed successfully, and requests immediate cancellation
    /// of the subscription with refund.
    /// </summary>
    public const string Cancelled = "Cancelled";

    /// <summary>
    /// Pending is applied to a Paid subscription when
    /// the billing date has arrived or recently passed,
    /// and the Payment has not yet been obtained.
    /// </summary>
    public const string Pending = "Pending";

    /// <summary>
    /// For Paid subscriptions, the user can "suspend"
    /// billing and access to the Paid features for a
    /// limited period of time.
    /// 
    /// During the time that an account is Suspended,
    /// any Resource Quotas are temporarily set to Zero,
    /// days remaining on their most recent billing cycle
    /// are retained, and the user is not billed.
    /// 
    /// Upon resuming the subscription, the next billdate
    /// is calculated, quotas are restored, and the user
    /// may resume regular usage of the software.
    /// </summary>
    public const string Suspended = "Suspended";
}
