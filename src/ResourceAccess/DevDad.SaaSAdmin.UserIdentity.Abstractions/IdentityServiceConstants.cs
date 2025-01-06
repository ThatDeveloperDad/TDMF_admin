using System;

namespace DevDad.SaaSAdmin.UserIdentity.Abstractions;

public class IdentityServiceConstants
{
    public static class ErrorKinds
    {
        public const string UnknownIdentity = "IdentityNotFound";
        public const string ExternalApiError = "ExternalError";
    }
}
