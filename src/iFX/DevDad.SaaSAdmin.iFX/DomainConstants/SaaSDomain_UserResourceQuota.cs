using System;

namespace DevDad.SaaSAdmin.iFX.DomainConstants;

public partial class SaaSDomain
{
    public static class UserQuotas
    {
        public const string EntityName = "UserQuota";
        public static class Attributes
        {
            public const string UserId = "UserId";
            public const string Storage = "Storage";
            public const string AiGenerations = "AiGenerations";
        }
    }

    public static class UserResourceQuota
    {
        public const string EntityName = "UserResourceQuota";
        public static class Attributes
        {
            public const string ResourceKind = "MeteredResourceKind";
            
            public const string GrantedBudget = "ResourceBudget";

            public const string ConsumedBudget = "ConsumedBudget";
        }
    }
}
