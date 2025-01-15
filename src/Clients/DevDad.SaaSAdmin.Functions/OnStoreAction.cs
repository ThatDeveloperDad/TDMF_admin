using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using DevDad.SaaSAdmin.Functions.ApiModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DevDad.SaaSAdmin.Functions
{
    public class OnStoreAction
    {
        private readonly ILogger<OnStoreAction> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public OnStoreAction(ILogger<OnStoreAction> logger,
            IHttpClientFactory httpClientFactory,
            IConfiguration config)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        [Function("OnStoreAction")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            string apiEndpointPath = "hooks/processStoreEvent";
            IActionResult? result = null;
            string rawBody;
            
            try
            {
                using(StreamReader reader = new StreamReader(req.Body))
                {
                    rawBody = await reader.ReadToEndAsync();
                }

                //Validate Request Signature.  If not valid, reject & return 401.
                bool signerCorrect = ValidateRequestSignature(req.Headers["X-Signature"], rawBody);
                
                if(signerCorrect == true)
                {
                    LsEventData eventData = new LsEventData()
                    {
                        EventJson = rawBody,
                        RequestId = req.HttpContext.TraceIdentifier
                    };

                    
                    HttpClient apiClient = CreateApiClient();
                    string url = apiClient.BaseAddress + apiEndpointPath;

                    var apiResponse = await apiClient.PostAsJsonAsync(url, eventData);

                    if(apiResponse.IsSuccessStatusCode)
                    {
                        result = new OkResult();
                    }
                    else
                    {
                        _logger?.LogError("An error occurred while processing the request.  Status Code: {0}  Message: {1}"
                            , apiResponse.StatusCode
                            , apiResponse.ReasonPhrase);
                        result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
                    }
                }
                else
                {
                    result = new UnauthorizedResult();
                }
            }
            catch (Exception e)
            {
                _logger?.LogError(e, "An error occurred while processing the request.");
                result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            

            _logger?.LogInformation("C# HTTP trigger function processed a request for OnStoreAction.");
            return result;
        }

        private HttpClient CreateApiClient()
        {
            string? baseUrl = _config["adminApi:baseUrl"];
            if(string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new InvalidOperationException("The base URL for the Admin API is not configured.");
            }

            HttpClient client =  _httpClientFactory.CreateClient("AdminApi");
            client.BaseAddress = new Uri(baseUrl);

            return client;
        }

        private bool ValidateRequestSignature(string? x_sig, string body)
        {
            if(string.IsNullOrWhiteSpace(x_sig))
            {
                _logger.LogWarning("X-Signature header is missing. Request is invalid.");
                return false;
            }

            bool signerCorrect = false;

            // TODO:  stuff the real value in .env / environment vars.
            // this is a test-only value and won't be used in production.
            string signingSecret = "GGEDpCdG,VVFq!YoTsL+)_+L|kT?N[";
            // Need to has the signing secret w/ HMAC SHA256
            using(var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(signingSecret)))
            {
                var digest = hmac.ComputeHash(Encoding.UTF8.GetBytes(body));
                var digestHex = BitConverter.ToString(digest).Replace("-", "").ToLower();
                var digestBytes = Encoding.UTF8.GetBytes(digestHex);

                var sigBytes = Encoding.UTF8.GetBytes(x_sig);

                signerCorrect = CryptographicOperations.FixedTimeEquals(sigBytes, digestBytes);
            }

            return signerCorrect;
        }
    }
}
