using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using VibeFunctionsIsolated.DAL.Interfaces;

namespace VibeFunctionsIsolated.Functions.Events
{
    public class UpsertEvent
    {
        private readonly ILogger<UpsertEvent> logger;
        private readonly ICosmosDataAccess cosmosDataAccess;

        public UpsertEvent(ILogger<UpsertEvent> logger, ICosmosDataAccess cosmosDataAccess)
        {
            this.logger = logger;
            this.cosmosDataAccess = cosmosDataAccess;
        }

        [Function("UpsertEvent")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
