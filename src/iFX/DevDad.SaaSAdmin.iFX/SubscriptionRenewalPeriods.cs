namespace DevDad.SaaSAdmin.iFX;

/// <summary>
/// Describes the number of days between subscription renewals.
/// 
/// </summary>
public class SubscriptionRenewalFrequencies
{
    public readonly int[] ValidFrequencies = [Permanent,Monthly];

    public const int Permanent = 0;
    public const int Monthly = 30;
}
