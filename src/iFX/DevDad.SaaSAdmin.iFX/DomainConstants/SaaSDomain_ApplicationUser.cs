namespace DevDad.SaaSAdmin.iFX.DomainConstants;

/// <summary>
/// SaaSDomain does nothing more than provide organized constants
/// that DESCRIBE the domain Model, and the Entities from which it is built.
/// </summary>
public partial class SaaSDomain
{
    public static class ApplicationUser
    {
        public const string EntityName = "ApplicationUser";

        public static class Attributes
        {
            public const string UserId = "UserId";
            public const string DisplayName = "DisplayName";
            public const string SubscriptionStatus = "SubscriptionStatus";

            public const string Subscription = "Subscription";

            public const string CorrelationIds = "CorrelationIds";
        }
    }
}
