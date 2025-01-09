using System;
using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.StoreManager.Contracts;

/// <summary>
/// Represents the request to the StoreManager carrying the data to begin a new checkout session.
/// </summary>
public class StartCheckoutRequest : OperationRequest<NewCheckoutData>
{
    public StartCheckoutRequest(string workloadName, NewCheckoutData? payload = null) : base(workloadName, payload)
    {
    }

    public override string OperationName => "RequestNewCheckoutSession";

    public NewCheckoutData? CheckoutData
    {
        get => Payload;
        set => Payload = value;
    }
}
