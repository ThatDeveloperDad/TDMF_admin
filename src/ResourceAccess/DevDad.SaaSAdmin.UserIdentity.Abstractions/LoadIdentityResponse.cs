using System;
using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.UserIdentity.Abstractions;

public class LoadIdentityResponse : OperationResponse<UserIdentityResource>
{
    public LoadIdentityResponse(OperationRequest request, UserIdentityResource? payload = null)
        : base(request, payload){}
}
