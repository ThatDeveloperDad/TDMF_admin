using System;

namespace DevDad.SaaSAdmin.API.PublicModels;

public class LoadProfileResponse
{
    public string UserId { get; set; } = string.Empty;
    public string DisplayName {get;set;} = string.Empty;
    public string  SubscriptionSku { get; set; } = string.Empty;
}
