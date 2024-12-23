using System;

namespace DevDad.SaaSAdmin.AccountManager.Contracts;

public class AccountManagerConstants
{
    public static class ModifySubscriptionErrors
    {
        public const string Validation_AddNewSameSku = "Cannot create a new subscription with the same SKU as the existing subscription.";
        public const string Validation_AddFreeToActivePaid = "Cannot overwrite an active paid subscirption with a new Free Subscription.";
        public const string Validation_ActivityNotValidForStatus = "The requested activity is not valid for the current subscription status.";

        public const string Validation_ActivityNotValidForSubscriptionType = "The requested activity is not valid for the current subscription type.";
    }
}
