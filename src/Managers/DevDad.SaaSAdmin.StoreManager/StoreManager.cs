using DevDad.SaaSAdmin.Catalog.Abstractions;
using DevDad.SaaSAdmin.Catalog.HardCoded;
using DevDad.SaaSAdmin.iFX;
using DevDad.SaaSAdmin.StoreAccess.Abstractions;
using DevDad.SaaSAdmin.StoreManager.Contracts;
using Microsoft.Extensions.Logging;
using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.StoreManager;

public class StoreManager : IStoreManager
{

    private readonly IStoreApiAccess _storeAccess;
    private readonly ICatalogAccess _catalogAccess;


    private readonly ILogger<StoreManager>? _logger;

    public StoreManager(IStoreApiAccess storeAccess,
        ICatalogAccess catalogAccess,
        ILoggerFactory? loggerFactory)
    {
        _storeAccess = storeAccess;
        _logger = loggerFactory?.CreateLogger<StoreManager>();
        _catalogAccess = catalogAccess;
    }


    public async Task<StartCheckoutResponse> StartCheckoutAsync(StartCheckoutRequest request)
    {
        string executionSite = $"Workload: {request.WorkloadName} | Operation: {request.OperationName} | ExecutionId: {request.WorkloadId}";
        StartCheckoutResponse response = new(request, null);

        var validationResult = ValidateStartCheckoutRequest(request);
        response.AddErrors(validationResult, executionSite);

        if(response.ShouldHaltProcess)
        {
            return response;
        }

        // Convert the MANAGER's RequestData to the DTO expected by the ApiAceess component.
        // Since this is a Single Method, until I implement other methods, I'mma do the mapping code
        // between the idiomatic types here.
        var subData = await GetTemplateForSku(request.CheckoutData!.RequestedSku);
        if(subData == null)
        {
            ServiceError err = new()
            {
                Site = executionSite,
                Message = "The requested SKU was not found in the catalog.",
                ErrorKind = ServiceErrorKinds.RequestPayloadValidation,
                Severity = ErrorSeverity.Error
            };
            response.AddError(err);
        }

        if(string.IsNullOrWhiteSpace(subData!.VendorItemId))
        {
            ServiceError err = new()
            {
                Site = executionSite,
                Message = "The requested SKU was found in the catalog, but is not administered through our Merchant service.",
                ErrorKind = ServiceErrorKinds.RequestPayloadValidation,
                Severity = ErrorSeverity.Error
            };
            response.AddError(err);
        }

        if(response.ShouldHaltProcess)
        {
            return response;
        }

        CheckoutData apiData = new()
        {
            CustomerEntraId = request.CheckoutData!.LocalUserId,
            ProductIdToPurchase = subData.VendorItemId!
        };

        CheckoutRequest apiRequest = new(request)
        {
            CheckoutData = apiData
        };
        
        var apiAccessResponse = await _storeAccess.StartCheckoutAsync(apiRequest);

        if(apiAccessResponse.Successful
            && apiAccessResponse.Payload != null)
        {
            response.Payload = apiAccessResponse.Payload;
        }
        else
        {
            response.AddErrors(apiAccessResponse);
        }

        return response;
    }

    private async Task<SubscriptionTemplateResource?> GetTemplateForSku(string sku)
    {
        SubscriptionTemplateResource? tplt = null;

        tplt = await _catalogAccess.GetCatalogItemAsync(sku);

        return tplt;
    }

    private IEnumerable<ServiceError> ValidateStartCheckoutRequest(StartCheckoutRequest request)
    {
        List<ServiceError> errors = new();


        return errors;
    }
}
