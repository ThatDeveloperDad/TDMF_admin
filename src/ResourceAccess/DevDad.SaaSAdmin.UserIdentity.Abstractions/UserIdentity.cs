using System;

namespace DevDad.SaaSAdmin.UserIdentity.Abstractions;

public record UserIdentity
{
    public UserIdentity()
    {
        UserId = string.Empty;
        DisplayName = string.Empty;
    }

    public UserIdentity(string userId, string displayName)
    {
        UserId = userId;
        DisplayName = displayName;
    }

    public string UserId { get; init; }
    public string DisplayName { get; init; }
}
