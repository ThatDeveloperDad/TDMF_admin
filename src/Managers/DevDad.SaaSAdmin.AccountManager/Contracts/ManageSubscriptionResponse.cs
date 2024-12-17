using System;
using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.AccountManager.Contracts;

public class ManageSubscriptionResponse : OperationResponse<bool?>
{
    public ManageSubscriptionResponse(OperationRequest request, bool? payload) : base(request, payload)
    {
    }

    public bool SubscriptionWasUpdated => Payload??false;
}
