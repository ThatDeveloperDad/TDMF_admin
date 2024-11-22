using System;

namespace DevDad.SaaSAdmin.UserIdentity.Abstractions;

public record UserIdentityResource
{
    public UserIdentityResource()
    {
        UserId = string.Empty;
        DisplayName = string.Empty;
    }

    public UserIdentityResource(string userId, string displayName)
    {
        UserId = userId;
        DisplayName = displayName;
    }

    public string UserId { get; init; }
    public string DisplayName { get; init; }
}
