using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using VibeFunctionsIsolated.DAL.Interfaces;
using VibeFunctionsIsolated.Models.Cosmos;
using VibeFunctionsIsolated.Utility.UtilityInterfaces;

namespace VibeFunctionsIsolated.Utility;

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

    public async Task<IEnumerable<CalendarEvent>> GetAllCalendarEvents()
    {
        const string query = "SELECT * FROM c";
        IEnumerable<CalendarEvent> calendarEvents = await cosmosDataAccess.GetAllItemsAsync<CalendarEvent>(query);

        return calendarEvents;
    }

    public async Task<bool> UpsertCalendarEvent(CalendarEvent calendarEvent)    
    {
        if (calendarEvent == null)
        {
            throw new ArgumentNullException(nameof(CalendarEvent), "Calendar event cannot be null");
        }
        bool didUpsert = false;

        ItemResponse<CalendarEvent> upsertResponse = await cosmosDataAccess.UpsertCosmosItemAsync(calendarEvent, calendarEvent.id);

        if (upsertResponse.StatusCode == System.Net.HttpStatusCode.OK)
        {
            logger.LogInformation("Item with id {0} updated in CosmosDB", calendarEvent.id);
            didUpsert = true;
        }
        else if (upsertResponse.StatusCode == System.Net.HttpStatusCode.Created)
        {
            logger.LogInformation("Item with id {0} created in CosmosDB", calendarEvent.id);
            didUpsert = true;
        } else
        {
            Type? upsertType = upsertResponse?.Resource.GetType();
            logger.LogError("Error updating or creating item with type {upsertType} in CosmosDB. StatusCode: {statusCode}", upsertType, upsertResponse?.StatusCode);
        }

        return didUpsert;
    }
}
