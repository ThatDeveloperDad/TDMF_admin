using System;

namespace DevDad.SaaSAdmin.iFX;

public class ExternalServiceVendors
{
    public readonly string[] AllowedValues = new string[]
    {
        MsEntra,
        LemonSqueezy
    };
    
    public const string MsEntra = "MicrosoftEntra";

    public const string LemonSqueezy = "LemonSqueezy";
}
