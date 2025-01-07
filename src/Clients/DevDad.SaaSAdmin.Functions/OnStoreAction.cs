using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace DevDad.SaaSAdmin.Functions
{
    public class OnStoreAction
    {
        private readonly ILogger<OnStoreAction> _logger;

        public OnStoreAction(ILogger<OnStoreAction> logger)
        {
            _logger = logger;
        }

        [Function("OnStoreAction")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            
            string rawBody;
            using(StreamReader reader = new StreamReader(req.Body))
            {
                rawBody = await reader.ReadToEndAsync();
            }

            bool signerCorrect = ValidateRequestSignature(req.Headers["X-Signature"], rawBody);
            

            string resultText = signerCorrect ? "Signature is valid." : "Signature is invalid.";
            
            _logger.LogInformation("C# HTTP trigger function processed a request for OnStoreAction.");
            return new OkObjectResult($"OnStoreAction processed. {resultText}");
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
