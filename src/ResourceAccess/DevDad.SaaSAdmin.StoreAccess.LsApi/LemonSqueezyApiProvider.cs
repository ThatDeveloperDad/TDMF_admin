using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using DevDad.SaaSAdmin.iFX;
using DevDad.SaaSAdmin.StoreAccess.Abstractions;
using DevDad.SaaSAdmin.StoreAccess.LsApi.LemonSqueezyModels;
using Microsoft.Extensions.Logging;
using ThatDeveloperDad.iFX.Serialization;
using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.StoreAccess.LsApi;

/// <inheritdoc cref="DevDad.SaaSAdmin.StoreAccess.Abstractions.IStoreAccess"/> 
/// <summary>
/// Implements StoreAccess for the LemonSqueezy ecommerce API.
/// </summary>
public class LemonSqueezyApiProvider : IStoreApiAccess
{

    private readonly LsApiOptions _options;
    private readonly ILogger<LemonSqueezyApiProvider>? _logger;

    private readonly HttpClient _httpClient;

    public LemonSqueezyApiProvider(LsApiOptions options,
        ILoggerFactory? loggerFactory,
        HttpClient httpClient)
    {
        _options = options;
        _logger = loggerFactory?.CreateLogger<LemonSqueezyApiProvider>();
        _httpClient = httpClient;
    }

    /// <inheritdoc />
    public async Task<CheckoutResponse> StartCheckoutAsync(CheckoutRequest request)
    {
        string executionSite = $"Workload: {request.WorkloadName} | Operation: {request.OperationName} | ExecutionId: {request.WorkloadId}";
        CheckoutResponse result = new(request, null);

        IEnumerable<ServiceError> errors = ValidateRequest(request);
        result.AddErrors(errors, executionSite);

        if(result.ShouldHaltProcess == true)
        {
            return result;
        }

        // We KNOW that this will evaluate to true, because we've already
        // validated that expectation.
        // If we hadn't checked for this specifically, we'd want to
        // put this in a conditional and possibly bail out of the operation.
        int.TryParse(request.CheckoutData!.ProductIdToPurchase, out int variantId);

        LsRequestBuilder requestBuilder = new LsRequestBuilder()
            .ForStore(_options.StoreId)
            .AsNewCheckout(
                    customerEntraId: request.CheckoutData!.CustomerEntraId,
                    productVariantId: variantId,
                    localSystemProductSku: request.CheckoutData.LocalSystemProductSku,
                    isTest: _options.IsTestMode
                );

        LsReqBody<LsCheckoutReq>? requestBody = requestBuilder
            .Build<LsCheckoutReq>(); 

        //Debug.WriteLine(requestBody.ToJson());

        string endpointFragment = ApiConstants.EndpointFragment_StartCheckout;
        string url = $"{_options.BaseUrl}{endpointFragment}";

        // Use the HttpClient to send the request to the LemonSqueezy API.
        // we need to include some additional Headers.

        HttpRequestMessage apiRequest = new HttpRequestMessage
            (
                method: HttpMethod.Post,
                requestUri: new Uri(url)
            );
        apiRequest.Headers.Add("Accept", "application/vnd.api+json");
        apiRequest.Headers.Add("Authorization", $"Bearer {_options.ApiKey}");
        apiRequest.Content = new StringContent(
            content: requestBody!.ToJson(),
            encoding: System.Text.Encoding.UTF8,
            mediaType: "application/vnd.api+json");


        var apiResponse = await _httpClient.SendAsync(apiRequest);
            
        if(apiResponse.IsSuccessStatusCode == true)
        {
            string responseJson = await apiResponse.Content.ReadAsStringAsync();
            string? returnedUrl = JsonUtilities.GetValueAtPath(responseJson, "$.data.attributes.url");

            result.CheckoutUrl = returnedUrl;
        }
        else
        {
            string responseContent = await apiResponse.Content.ReadAsStringAsync();
            _logger?.LogDebug(responseContent);

            string errMsg = $"Status: {apiResponse.StatusCode}  Reason:{apiResponse.ReasonPhrase}";
            ServiceError apiError = new()
            {
                ErrorKind = ServiceErrorKinds.ApiError,
                Message = errMsg,
                Severity = ErrorSeverity.Error,
                Site = executionSite + " | ApiRequest:" + ApiConstants.EndpointFragment_StartCheckout
            };
            result.AddError(apiError);
        }

        return result;
    }



    private IEnumerable<ServiceError> ValidateRequest<T>(OperationRequest<T> request)
        where T: class
    {
        List<ServiceError> errors = new();

        if(request.Payload == null)
        {
            var error = new ServiceError()
            {
                ErrorKind = ServiceErrorKinds.RequestPayloadValidation,
                Message = "The Request Payload is missing.",
                Severity = ErrorSeverity.Error
            };

            errors.Add(error);
            return errors;
        }

        var payloadErrors = ValidateRequestData(request.Payload);
        errors.AddRange(payloadErrors);

        return errors;
    }

    private IEnumerable<ServiceError> ValidateRequestData<T>(T data)
        where T: class
    {
        List<ServiceError> errors = new();

        switch (typeof(T))
        {
            case Type t when t == typeof(CheckoutData):
                var checkoutErrors = ValidateCheckoutData(data as CheckoutData);
                errors.AddRange(checkoutErrors);
                break;
            default:
                var error = new ServiceError()
                {
                    ErrorKind = ServiceErrorKinds.RequestPayloadValidation,
                    Message = "The provided Payload Type is not supported.",
                    Severity = ErrorSeverity.Error
                };
                
                errors.Add(error);
                break;
        }

        return errors;
    }

    private IEnumerable<ServiceError> ValidateCheckoutData(CheckoutData? data)
    {
        List<ServiceError> errors = new();

        if(data == null)
        {
            var error = new ServiceError()
            {
                ErrorKind = ServiceErrorKinds.RequestPayloadValidation,
                Message = "The CheckoutData is missing.",
                Severity = ErrorSeverity.Error
            };

            errors.Add(error);
            return errors;
        }

        //TODO:  Add more variable checking.  We need to make sure that the
        // inbound data is complete, and as correct as we can manage here.
        if(string.IsNullOrWhiteSpace(data.CustomerEntraId))
        {
            var error = new ServiceError()
            {
                ErrorKind = ServiceErrorKinds.RequestPayloadValidation,
                Message = "The CustomerEntraId is required.",
                Severity = ErrorSeverity.Error
            };

            errors.Add(error);
        }

        if(string.IsNullOrWhiteSpace(data.ProductIdToPurchase))
        {
            var error = new ServiceError()
            {
                ErrorKind = ServiceErrorKinds.RequestPayloadValidation,
                Message = "The ProductIdToPurchase is required.",
                Severity = ErrorSeverity.Error
            };

            errors.Add(error);
        }

        if(int.TryParse(data.ProductIdToPurchase, out int _)==false)
        {
            var error = new ServiceError()
            {
                ErrorKind = ServiceErrorKinds.RequestPayloadValidation,
                Message = "LemonSqueezy requires an integer value for ProductId.",
                Severity = ErrorSeverity.Error
            };

            errors.Add(error);
        }
        
        return errors;
    }

}