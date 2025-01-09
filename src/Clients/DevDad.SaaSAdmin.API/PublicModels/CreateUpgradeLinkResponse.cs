using System;
using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.API.PublicModels;

public class CreateUpgradeLinkResponse : OperationResponse<string?>
{
    public CreateUpgradeLinkResponse(OperationRequest request, string? payload) : base(request, payload)
    {
    }

    public string? PersonalizedCheckoutUrl { get; set; }
}
