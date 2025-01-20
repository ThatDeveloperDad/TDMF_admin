using System;
using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.StoreAccess.LsApi;

/// <summary>
/// Carries configuration data required to connect to the LemonSqueezy API.
/// </summary>
public class LsApiOptions : IServiceOptions
{
    public string BaseUrl { get; set; } = string.Empty;

    public string ApiKey { get; set; } = string.Empty;

    public int StoreId { get; set; }

    public bool IsTestMode { get; set; }
}
