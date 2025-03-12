using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using VibeFunctionsIsolated.DAL.Interfaces;
using VibeFunctionsIsolated.Models.Cosmos;
using VibeFunctionsIsolated.Utility.UtilityInterfaces;

namespace VibeFunctionsIsolated.Functions.Events
{
    public class UpsertEvent(ILogger<UpsertEvent> logger, ICosmosDataAccess cosmosDataAccess, IApplicationUtility applicationUtility)
    {
        private readonly ILogger<UpsertEvent> logger = logger;
        private readonly ICosmosDataAccess cosmosDataAccess = cosmosDataAccess;
        private readonly string containerName = "Events";
        private readonly IApplicationUtility applicationUtility = applicationUtility;

        [Function("UpsertEvent")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            cosmosDataAccess.ChangeContainerName(containerName);

            CalendarEvent? calendarEvent = await applicationUtility.DeserializeStream<CalendarEvent>(req.Body);

            if (calendarEvent == null)
            {
                string upsertEventClass = nameof(UpsertEvent);
                logger.LogError("{upsertEventClass}: Invalid request body", upsertEventClass);
                return new BadRequestObjectResult("Invalid request body");
            }

            CalendarEvent upsertedEvent = await cosmosDataAccess.UpsertItemAsync(calendarEvent.EventId, calendarEvent);

            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
