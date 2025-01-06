using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace DevDad.SaaSAdmin.Functions
{
    public class OnNewEntraSignup
    {
        private readonly ILogger<OnNewEntraSignup> _logger;

        public OnNewEntraSignup(ILogger<OnNewEntraSignup> logger)
        {
            _logger = logger;
        }

        [Function("OnNewEntraSignup")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request for OnNewEntraSignup.");
            return new OkObjectResult("NewEntraSignup executed.");
        }
    }
}
