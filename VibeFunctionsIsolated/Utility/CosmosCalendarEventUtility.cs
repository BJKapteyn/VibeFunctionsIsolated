using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using VibeFunctionsIsolated.DAL.Interfaces;
using VibeFunctionsIsolated.Models.Cosmos;
using VibeFunctionsIsolated.Models.Interfaces;
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

        public async Task<CalendarEvent?> UpsertCalendarEvent(CalendarEvent calendarEvent)
        {
            if (calendarEvent == null)
            {
                throw new ArgumentNullException(nameof(CalendarEvent), "Calendar event cannot be null");
            }

            ItemResponse<CalendarEvent> upsertResponse = await cosmosDataAccess.UpsertCosmosItemAsync(calendarEvent, calendarEvent.id);

            if (upsertResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                logger.LogInformation("Item with {updateItemId} updated in CosmosDB", upsertResponse);
            }
            else if (upsertResponse.StatusCode == System.Net.HttpStatusCode.Created)
            {
                logger.LogInformation("Item with {updateItemId} created in CosmosDB", upsertResponse);
            }
            else if (upsertResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                logger.LogWarning("Item with {updateItemId} not found in CosmosDB", upsertResponse);
            }
            else
            {
                Type? upsertType = upsertResponse?.Resource.GetType();
                logger.LogError("Error updating item with type {upsertType} in CosmosDB. StatusCode: {statusCode}", upsertType, upsertResponse?.StatusCode);
            }

            return upsertResponse?.Resource;
        }
    }
}
