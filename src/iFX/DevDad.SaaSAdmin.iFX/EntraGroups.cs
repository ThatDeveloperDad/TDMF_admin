using System;

namespace DevDad.SaaSAdmin.iFX;

/// <summary>
/// Describes the Groups as defined in MS-Entra that are relevant to users of
/// This SaaS App, that CAN BE manipulated via the Admin System.
/// </summary>
public class EntraGroups
{
    public const string AppGroupPrefix = "app-gmtools-";
    public readonly string[] ValidGroups = [FreeUsers,PaidUsers];

    public const string FreeUsers = "app-gmtools-users";
    public const string PaidUsers = "app-gmtools-paid-users";
}
