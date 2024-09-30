using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace VibeCollectiveFunctions.Items
{
    public class GetItemCategories
    {
        private readonly ILogger<GetItemCategories> _logger;

        public GetItemCategories(ILogger<GetItemCategories> logger)
        {
            _logger = logger;
        }

        [Function("GetItemCategories")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation($"C# HTTP {nameof(GetItemCategories)} trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
