using System;

namespace DevDad.SaaSAdmin.UserAccountAccess.Abstractions;

public class UserIdResource
{
    public string VendorName { get; set; } = string.Empty;
    public string UserIdAtVendor { get; set; } = string.Empty;
}
