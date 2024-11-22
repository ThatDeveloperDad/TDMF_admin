using System;
using System.Reflection.Metadata.Ecma335;
using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.UserIdentity.Abstractions;

public sealed class LoadIdentityRequest : OperationRequest<string>
{
    public override string OperationName => "LoadUserIdentity";

    public LoadIdentityRequest(OperationRequest parent, string? Payload = null) 
        : base(parent, Payload)
    {
    }

    public string? UserId 
    { 
        get=> Payload; 
        set=> Payload = value; 
    }

    
}
