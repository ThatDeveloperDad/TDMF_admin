using System;
using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.AccountManager.Contracts;

public class ManageSubscriptionRequest : OperationRequest<SubscriptionActionRequest>
{
    private readonly string _operationName = "ManageCustomerSubscription";

    public ManageSubscriptionRequest(string workloadName, SubscriptionActionRequest? payload = null) : base(workloadName, payload)
    {
    }

    public override string OperationName => _operationName;
}
