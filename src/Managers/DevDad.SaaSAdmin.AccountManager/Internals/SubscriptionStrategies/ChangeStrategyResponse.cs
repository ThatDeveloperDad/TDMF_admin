using System;
using DevDad.SaaSAdmin.AccountManager.Contracts;
using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.AccountManager.Internals.SubscriptionStrategies;

internal class ChangeStrategyResponse : OperationResponse<CustomerSubscription>
{
    public ChangeStrategyResponse(OperationRequest request, CustomerSubscription? payload) : base(request, payload)
    {
    }

    public bool ChangeCompleted { get; set; }
}
