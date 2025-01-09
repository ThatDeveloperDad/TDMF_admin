using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.StoreAccess.Abstractions;

public class CheckoutRequest : OperationRequest<CheckoutData>
{
    public CheckoutRequest(string workloadName, CheckoutData? payload = null) : base(workloadName, payload)
    {
    }

    public override string OperationName => "InvokeStoreAPI:CreateCheckout";

    public CheckoutData? CheckoutData 
    { 
        get => Payload;
        set => Payload = value; 
    }
}
