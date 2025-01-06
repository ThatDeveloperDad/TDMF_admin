using System;

namespace DevDad.SaaSAdmin.UserIdentity.Abstractions;

public class ReconcileMembershipsData
{
    public ReconcileMembershipsData()
    {
        UserId = string.Empty;
        ExpectedGroups = Array.Empty<string>();
    }

    public string UserId { get; set; }

    public string[] ExpectedGroups { get; set; }
}
