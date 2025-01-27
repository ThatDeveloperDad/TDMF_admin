using System;
using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.UserIdentity.Abstractions;

public class ReconcileMembershipsRequest : OperationRequest<ReconcileMembershipsData>
{
    private const string _operationName = "ReconcileAuthorizationGroups";

    public ReconcileMembershipsRequest(string workloadName, ReconcileMembershipsData? payload = null) : base(workloadName, payload)
    {
    }

    public ReconcileMembershipsRequest(OperationRequest parent, ReconcileMembershipsData? payload = null) : base(parent, payload)
    {
    }

    public override string OperationName => _operationName;
}
