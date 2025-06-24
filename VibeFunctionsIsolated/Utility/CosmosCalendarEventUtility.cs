using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using VibeFunctionsIsolated.DAL.Interfaces;
using VibeFunctionsIsolated.Models.Cosmos;
using VibeFunctionsIsolated.Utility.UtilityInterfaces;

namespace VibeFunctionsIsolated.Utility
{
    public class CosmosCalendarEventUtility : ICosmosCalendarEventUtility
    {
        private readonly ICosmosDataAccess cosmosDataAccess;
        private readonly ILogger<CosmosCalendarEventUtility> logger;

        public CosmosCalendarEventUtility(ICosmosDataAccess cosmosDataAccess, ILogger<CosmosCalendarEventUtility> logger)
        {
            this.cosmosDataAccess = cosmosDataAccess;
            this.logger = logger;

            cosmosDataAccess.ChangeContainerName("Events");
        }

        public async Task<IEnumerable<CalendarEvent>> MapAllCalendarEventsFromResponse()
        {
            const string query = "SELECT * FROM c";
            IEnumerable<CalendarEvent> calendarEvents = await cosmosDataAccess.GetAllItemsAsync<CalendarEvent>(query);
            return calendarEvents;
        }

        public async Task<IActionResult> UpsertCalendarEvent(CalendarEvent calendarEvent)
        {
            if (calendarEvent == null)
            {
                throw new ArgumentNullException(nameof(CalendarEvent), "Calendar event cannot be null");
            }

            ItemResponse<CalendarEvent> upsertResponse = await cosmosDataAccess.UpsertCosmosItemAsync(calendarEvent, calendarEvent.id);

            if (upsertResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                logger.LogInformation("Item with id {0} updated in CosmosDB", calendarEvent.id);
                
                return new OkResult();
            }
            else if (upsertResponse.StatusCode == System.Net.HttpStatusCode.Created)
            {
                logger.LogInformation("Item with id {0} created in CosmosDB", calendarEvent.id);

                return new CreatedResult();
            }

            Type? upsertType = upsertResponse?.Resource.GetType();
            logger.LogError("Error updating or creating item with type {upsertType} in CosmosDB. StatusCode: {statusCode}", upsertType, upsertResponse?.StatusCode);

            return new BadRequestResult();
        }

        //private IActionResult CreateResponseCode(System.Net.HttpStatusCode statusCode, string crudMethodName)
        //{
           
        //    return statusCode switch
        //    {
        //        System.Net.HttpStatusCode.OK => new OkObjectResult(resource),
        //        System.Net.HttpStatusCode.Created => new CreatedResult($"/{resource}", resource),
        //        System.Net.HttpStatusCode.NotFound => new NotFoundObjectResult(resource),
        //        _ => new StatusCodeResult((int)statusCode)
        //    };
        //}
    }
}
