using System;
using Microsoft.Extensions.Caching.Memory;

namespace DevDad.SaaSAdmin.API.ApiServices;

/// <summary>
/// This service is intended to cache inbound webhook requests for a few minutes to prevent double-processing.
/// For ome account management use cases, multiple requests are made by the ECommerceProvider,
/// and those requests may be redundant.
/// </summary>
public class WebhookRequestCache
{
    private const int CacheDurationMinutes = 5;
    private readonly IMemoryCache _cache;

    public WebhookRequestCache(IMemoryCache cache)
    {
        _cache = cache;
    }

    /// <summary>
    /// Checks the memory cache for the Subscription and Action, returns whether it's
    /// present or not, and adds it if it's not.
    /// </summary>
    /// <param name="subscriptionId"></param>
    /// <param name="actionName"></param>
    public bool CheckActionIsProcessing(string subscriptionId, string actionName)
    {
        bool isPresent = false;
        var key = $"{subscriptionId}-{actionName}";

        if (_cache.TryGetValue(key, out _))
        {
            isPresent = true;
        }
        else
        {
            _cache.Set(key, DateTime.UtcNow, TimeSpan.FromMinutes(CacheDurationMinutes));
            isPresent = false;
        }

        return isPresent;
    }


}
