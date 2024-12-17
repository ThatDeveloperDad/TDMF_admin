using System;
using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.AccountManager.Contracts;

public class ManageSubscriptionRequest : OperationRequest<SubscriptionActionDetail>
{
    private readonly string _operationName = "ManageCustomerSubscription";

    public ManageSubscriptionRequest(string workloadName, SubscriptionActionDetail? payload = null) : base(workloadName, payload)
    {
    }

    public override string OperationName => _operationName;

    public SubscriptionActionDetail? RequestDetail => Payload;

    public string? CustomerProfileId => RequestDetail?.CustomerProfileId;
}
