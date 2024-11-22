using System;
using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.AccountManager.Internals;

internal class BuildProfileRequest : OperationRequest<string>
{
    public BuildProfileRequest(OperationRequest parentRequest, string? payload = null) 
        : base(parentRequest, payload)
    {
    }

    /// <summary>
    /// Convenience property to expose the Payload in a friendlier manner.
    /// 
    /// Doing this is completely optional, I'm only doing it here for readability.
    /// </summary>
    public string? UserId
    {
        get => Payload;
        set => Payload = value;
    }

    public override string OperationName => "LoadAndBuildCustomer";
}
