using System;
using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.StoreManager.Contracts;

public class StartCheckoutResponse : OperationResponse<string?>
{
    public StartCheckoutResponse(OperationRequest request, string? payload) : base(request, payload)
    {
    }

    public string? CheckoutUrl
    {
        get => Payload;
        set => Payload = value;
    }
}
