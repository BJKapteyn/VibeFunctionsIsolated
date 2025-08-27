using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using VibeFunctionsIsolated.DAL.Interfaces;
using VibeFunctionsIsolated.Models.Cosmos;
using VibeFunctionsIsolated.Models.Interfaces;
using VibeFunctionsIsolated.Utility.UtilityInterfaces;

namespace VibeFunctionsIsolated.Utility;

public class CalendarEventUtility : ICalendarEventUtility
{
    private readonly ICosmosDataAccess cosmosDataAccess;
    private readonly ILogger<CalendarEventUtility> logger;

    public CalendarEventUtility(ICosmosDataAccess cosmosDataAccess, ILogger<CalendarEventUtility> logger)
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
            throw new ArgumentNullException(nameof(calendarEvent), "Calendar event cannot be null");
        }
        bool didUpsert = false;

        CosmosResponse upsertResponse = await cosmosDataAccess.UpsertCosmosItemAsync(calendarEvent);

        if (upsertResponse.StatusCode == System.Net.HttpStatusCode.OK)
        {
            logger.LogInformation("Item with id {id} updated in CosmosDB", calendarEvent.id);
            didUpsert = true;
        }
        else if (upsertResponse.StatusCode == System.Net.HttpStatusCode.Created)
        {
            logger.LogInformation("Item with id {id} created in CosmosDB", calendarEvent.id);
            didUpsert = true;
        } else
        {
            Type? upsertType = upsertResponse?.CosmosItem.GetType();
            logger.LogError("Error updating or creating item with type {upsertType} in CosmosDB. StatusCode: {statusCode}", upsertType, upsertResponse?.StatusCode);
        }

        return didUpsert;
    }
    public async Task<bool> DeleteCalendarEvent(string id, string partitionKey)
    {
        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(partitionKey))
        {
            logger.LogError("DeleteCalendarEvent called with invalid id or partitionKey");
            return false;
        }

        try
        {
            ItemResponse<ICosmosItem> response = await cosmosDataAccess.DeleteCosmosItemAsync<ICosmosItem>(id, partitionKey);

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent ||
                response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                logger.LogInformation("CalendarEvent with id {id} deleted from CosmosDB", id);
                return true;
            }
            else
            {
                logger.LogWarning("Failed to delete CalendarEvent with id {id}. StatusCode: {statusCode}", id, response.StatusCode);
                return false;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred while deleting CalendarEvent with id {id}", id);
            return false;
        }
    }


}
