using DevDad.SaaSAdmin.StoreAccess.Abstractions;
using DevDad.SaaSAdmin.StoreManager.Contracts;
using Microsoft.Extensions.Logging;

namespace DevDad.SaaSAdmin.StoreManager;

public class StoreManager : IStoreManager
{

    private readonly IStoreApiAccess _storeAccess;
    private readonly ILogger<StoreManager>? _logger;

    public StoreManager(IStoreApiAccess storeAccess
    , ILoggerFactory? loggerFactory)
    {
        _storeAccess = storeAccess;
        _logger = loggerFactory?.CreateLogger<StoreManager>();
    }


    public Task<StartCheckoutResponse> StartCheckoutAsync(StartCheckoutRequest request)
    {
        throw new NotImplementedException();
    }
}
