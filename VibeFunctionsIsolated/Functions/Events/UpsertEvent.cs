using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using VibeFunctionsIsolated.DAL.Interfaces;
using VibeFunctionsIsolated.Models.Cosmos;
using VibeFunctionsIsolated.Utility.UtilityInterfaces;

namespace VibeFunctionsIsolated.Functions.Events
{
    public class UpsertEvent
    {
        private readonly ILogger<UpsertEvent> logger;
        private readonly ICosmosDataAccess cosmosDataAccess;
        private readonly string containerName = "Events";
        private readonly IApplicationUtility applicationUtility;

        public UpsertEvent(ILogger<UpsertEvent> logger, ICosmosDataAccess cosmosDataAccess, IApplicationUtility applicationUtility)
        {
            this.logger = logger;
            this.cosmosDataAccess = cosmosDataAccess;
            this.applicationUtility = applicationUtility;
        }

        [Function("UpsertEvent")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            CalendarEvent? @event = await applicationUtility.DeserializeStream<CalendarEvent>(req.Body);

            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
