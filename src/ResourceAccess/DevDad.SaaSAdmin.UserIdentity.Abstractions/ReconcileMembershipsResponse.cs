using System;
using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.UserIdentity.Abstractions;

public class ReconcileMembershipsResponse : OperationResponse<ReconcileMembershipResult>
{
    public ReconcileMembershipsResponse(OperationRequest request, ReconcileMembershipResult payload) : base(request, payload)
    {
    }

    public bool Completed
    {
        get
        {
            return Payload?.Succeeded ?? false;
        }
        set
        {
            if (Payload == null)
            {
                Payload = new ReconcileMembershipResult();
            }
            Payload.Succeeded = value;
        }
    }
    public int MembershipsAdded
    {
        get
        {
            return Payload?.MembershipsAdded ?? 0;
        }
        set
        {
            if (Payload == null)
            {
                Payload = new ReconcileMembershipResult();
            }
            Payload.MembershipsAdded = value;
        }
    }
    public int MembershipsRemoved
    {
        get
        {
            return Payload?.MembershipsRemoved ?? 0;
        }
        set
        {
            if (Payload == null)
            {
                Payload = new ReconcileMembershipResult();
            }
            Payload.MembershipsRemoved = value;
        }
    }
}
