using System;

namespace DevDad.SaaSAdmin.AccountManager.Internals;

/// <summary>
/// This class contains constants used to create errors raised by the Builder,
/// and to identify these errors when inspecting them in the Manager.
/// </summary>
internal class CustomerBuilderErrors
{
    public const string UserIdentity_NotFound = "UserIdentityNotFound";
    public const string UserProfile_NotFound = "UserProfileNotFound";
}
