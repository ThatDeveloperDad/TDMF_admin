using System;

namespace DevDad.SaaSAdmin.iFX.DomainConstants;

public partial class SaaSDomain
{
    public static class ExternalUserId
    {
        public const string EntityName = "ExternalUserId";

        public static class Attributes
        {
            public const string RemoteSystemName = "RemoteSystemName";
            public const string IdAtRemote = "IdAtRemote";
        }
    }
}
