using System;

namespace DevDad.SaaSAdmin.iFX.DomainConstants;

public partial class SaaSDomain
{
    public static class ApplicationUser
    {
        public const string EntityName = "ApplicationUser";

        public static class Attributes
        {
            public const string UserId = "UserId";
            public const string DisplayName = "DisplayName";
        }
    }
}
