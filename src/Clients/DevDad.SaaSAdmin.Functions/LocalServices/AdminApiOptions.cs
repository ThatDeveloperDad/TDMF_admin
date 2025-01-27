using System;
using System.Linq;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Logging;

namespace DevDad.SaaSAdmin.Functions.LocalServices;

public class AdminApiOptions
{
    public string TenantId { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;

    public string RequiredScope { get; set; } = string.Empty;

    public string ApiBaseUrl { get; set; } = string.Empty;

    internal static async Task<string> GetTokenAsync(
        AdminApiOptions options,
        ILogger? logger = null)
    {
        string authToken = string.Empty;
        try
        {
            
        var credential = new ClientSecretCredential(
            options.TenantId,
            options.ClientId,
            options.ClientSecret);
        
        var token = await credential.GetTokenAsync(
            new TokenRequestContext(new[] { options.RequiredScope }));
            authToken = token.Token;
        }
        catch(Exception ex){
            logger?.LogError(ex, "Failed to get token");
        }

        return authToken;
    }
}
