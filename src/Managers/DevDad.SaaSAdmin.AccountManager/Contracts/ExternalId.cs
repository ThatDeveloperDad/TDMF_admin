using System;

namespace DevDad.SaaSAdmin.AccountManager.Contracts;

public class ExternalId
{
    public string Vendor { get; set; } = string.Empty;
    public string IdAtVendor { get; set; } = string.Empty;
}
