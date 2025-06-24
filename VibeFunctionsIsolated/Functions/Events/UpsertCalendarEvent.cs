using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using VibeFunctionsIsolated.DAL.Interfaces;
using VibeFunctionsIsolated.Models.Cosmos;
using VibeFunctionsIsolated.Utility.UtilityInterfaces;

namespace VibeFunctionsIsolated.Functions.Events
{
    public class UpsertCalendarEvent(
            ILogger<UpsertCalendarEvent> logger,
            ICosmosDataAccess cosmosDataAccess,
            IApplicationUtility applicationUtility,
            ICosmosCalendarEventUtility cosmosCalendarEventUtility)
    {
        private readonly string containerName = "Events";
        private readonly ILogger<UpsertCalendarEvent> logger = logger;
        private readonly ICosmosDataAccess cosmosDataAccess = cosmosDataAccess;
        private readonly IApplicationUtility applicationUtility = applicationUtility;
        private readonly ICosmosCalendarEventUtility cosmosCalendarEventUtility = cosmosCalendarEventUtility;

        [Function("UpsertCalendarEvent")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            cosmosDataAccess.ChangeContainerName(containerName);

            CalendarEvent? calendarEvent = await applicationUtility.DeserializeStream<CalendarEvent>(req.Body);

            if (calendarEvent == null)
            {
                string upsertEventClass = nameof(UpsertCalendarEvent);
                logger.LogError("{upsertEventClass}: Invalid request body", upsertEventClass);
                return new BadRequestObjectResult("Invalid request body");
            }

            CalendarEvent upsertedEvent = await cosmosDataAccess.UpsertCosmosItemAsync(calendarEvent, calendarEvent.EventId);

            IActionResult result = await cosmosCalendarEventUtility.UpsertCalendarEvent(upsertedEvent);

            return new OkResult();
        }
    }
}
