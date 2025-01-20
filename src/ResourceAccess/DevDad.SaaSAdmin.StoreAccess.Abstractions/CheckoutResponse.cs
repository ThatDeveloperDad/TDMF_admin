using System;
using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.StoreAccess.Abstractions;

public class CheckoutResponse : OperationResponse<string?>
{
    public CheckoutResponse(OperationRequest request, string? payload) : base(request, payload)
    {
    }

    public string? CheckoutUrl 
    { 
        get => Payload; 
        set => Payload = value; 
    }
}
