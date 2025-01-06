using System;

namespace DevDad.SaaSAdmin.UserIdentity.Abstractions;

public class ReconcileMembershipResult
{
    public bool Completed { get; set; }

    public int? MembershipsAdded { get; set; }

    public int? MembershipsRemoved { get; set; }
}
