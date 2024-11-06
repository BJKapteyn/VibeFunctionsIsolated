using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace VibeFunctionsIsolated.Functions.Items
{
    public class GetProductItems
    {
        private readonly ILogger<GetProductItems> _logger;

        public GetProductItems(ILogger<GetProductItems> logger)
        {
            _logger = logger;
        }

        [Function("GetProductItems")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
