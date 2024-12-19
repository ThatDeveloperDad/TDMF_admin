using System;

namespace DevDad.SaaSAdmin.iFX;

public class SubscriptionIdentifiers
{
    public static readonly string[] AllowedSkus = 
    {
        SKUS_TDMF_PAID_MONTHLY,
        SKUS_TDMF_FREE
    };

    public const string SKUS_TDMF_PAID_MONTHLY = "DM-FAMILIAR-PD-MONTHLY";
    public const string SKUS_TDMF_FREE = "DM-FAMILIAR-FREE";
}
