using System;
using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.AccountManager.Contracts;

public class CustomerProfileResponse
    : OperationResponse<CustomerProfile>
{
    public CustomerProfileResponse(OperationRequest request, CustomerProfile? payload = null)
        : base(request, payload) {}
}
