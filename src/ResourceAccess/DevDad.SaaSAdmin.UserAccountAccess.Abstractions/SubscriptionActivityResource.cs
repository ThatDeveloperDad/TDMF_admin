using System;

namespace DevDad.SaaSAdmin.UserAccountAccess.Abstractions;

public class SubscriptionActivityResource
{
        public string ActivityKind { get; set; } = string.Empty;

        public DateTime ActivityDateUTC { get; set; } = DateTime.UtcNow;
}
