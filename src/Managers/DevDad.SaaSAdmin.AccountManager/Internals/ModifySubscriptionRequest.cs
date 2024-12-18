using System;
using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.AccountManager.Internals;

internal class ModifySubscriptionRequest : OperationRequest<ModifySubscriptionData>
{
    private const string _operationName = "ModifySubscription";
    public override string OperationName => _operationName;
    public ModifySubscriptionRequest(OperationRequest parent, ModifySubscriptionData? payload = null) : base(parent, payload)
    {
    }
}
