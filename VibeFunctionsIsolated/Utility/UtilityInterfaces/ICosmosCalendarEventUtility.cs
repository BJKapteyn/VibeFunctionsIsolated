using VibeFunctionsIsolated.Models.Cosmos;

namespace VibeFunctionsIsolated.Utility.UtilityInterfaces;

/// <summary>
/// Business logic layer for calendar event crud operations
/// </summary>
public interface ICosmosCalendarEventUtility
{
    /// <summary>
    /// Maps all calendar events from the Cosmos DB response to an  of CalendarEvent objects.
    /// </summary>
    /// <returns>Collection of calendar events from cosmosdb</returns>
    Task<IEnumerable<CalendarEvent>> GetAllCalendarEvents();


    /// <summary>
    /// Upsert Calendar Event to Cosmos DB.
    /// </summary>
    /// <param name="calendarEvent">Event to upsert</param>
    /// <returns>Calendar event it upserted</returns>
    Task<bool> UpsertCalendarEvent(CalendarEvent calendarEvent);
}
