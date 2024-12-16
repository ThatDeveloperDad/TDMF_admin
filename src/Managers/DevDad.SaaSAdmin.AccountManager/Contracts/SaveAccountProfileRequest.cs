using System;
using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.AccountManager.Contracts;

public class SaveAccountProfileRequest : OperationRequest<CustomerProfile>
{
    private readonly string _operationName = "SaveAccountProfile";

    public SaveAccountProfileRequest(string workloadName, CustomerProfile? payload = null) : base(workloadName, payload) { }

    public override string OperationName => _operationName;

    // Optional helper pass-through to make this easier to use in the Manager that will receive it.
    public CustomerProfile? Profile
    {
        get => Payload;
        set => Payload = value;
    }
}
