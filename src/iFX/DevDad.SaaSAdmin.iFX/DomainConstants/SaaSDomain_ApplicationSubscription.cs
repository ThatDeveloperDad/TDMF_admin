namespace DevDad.SaaSAdmin.iFX.DomainConstants;

public partial class SaaSDomain
{
    public static class ApplicationSubscription
    {
        public const string EntityName = "ApplicationSubscription";

        public static class Attributes
        {
            public const string UserId = "UserId";

            public const string SKU = "SKU";

            public const string CurrentPeriodStartUtc = "StartDateUtc";

            public const string CurrentPeriodEndUtc = "EndDateUtc";

            public const string WillRenew = "WillRenew";

            public const string CurrentStatus = "CurrentStatus";

            public const string Quotas = "Quotas";

            public const string SubscriptionHistory = "History";
        }
    }

    public static class SubscriptionHistoryEntry
    {
        public const string EntityName = "SubscriptionActivity";

        public static class Attributes
        {
            public const string ActivityDateUtc = "ActivityDateUtc";

            public const string ActivityKind = "ActivityKind";

            public const string Comment = "Comment";
        }
    }
}
