using System;

namespace DevDad.SaaSAdmin.iFX;

public class ExternalServiceVendors
{
    public static readonly string[] AllowedValues = new string[]
    {
        MsEntra,
        LemonSqueezy,
        ThatDeveloperDad
    };
    
    public const string ThatDeveloperDad = "ThatDeveloperDad";

    public const string MsEntra = "MicrosoftEntra";

    public const string LemonSqueezy = "LemonSqueezy";
}

/// <summary>
/// Contains constants that identify the systems that can originate a request to
/// change a Customer's Subscription.
/// </summary>
public class ChangeRequestSource
{
    public static readonly string[] AllowedValues = new string[]
    {
        VendorWebHook,
        AdminBackend
    };

    /// <summary>
    /// Identifies the entry point for a ChangeRequest as a Vendor WebHook.
    /// </summary>
    public const string VendorWebHook = "VendorWebHook";

    /// <summary>
    /// Identifies the entry point for a ChangeRequest as the Admin Backend. (this system or one of its clients)
    /// </summary>
    public const string AdminBackend = "AdminBackend";
}