using System;
using DevDad.SaaSAdmin.AccountManager.Contracts;
using DevDad.SaaSAdmin.UserIdentity.Abstractions;

namespace DevDad.SaaSAdmin.AccountManager.Internals;

internal static class CustomerProfileMappingExtensions
{
    public static CustomerProfile ApplyIdentityFrom(this CustomerProfile? profile, UserIdentityResource identityResource)
    {
        if(profile == null)
        {
            profile = new();
        }

        profile.UserId = identityResource.UserId;
        profile.DisplayName = identityResource.DisplayName;
        
        return profile;
    }
}
