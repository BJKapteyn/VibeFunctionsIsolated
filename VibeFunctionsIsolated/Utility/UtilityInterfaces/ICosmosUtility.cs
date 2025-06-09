using System.Collections.Generic;
using System.Threading.Tasks;
using VibeFunctionsIsolated.Models.Cosmos;

namespace VibeFunctionsIsolated.Utility.UtilityInterfaces
{
    public interface ICosmosUtility
    {
        /// <summary>
        /// Maps all calendar events from the Cosmos DB response to an  of CalendarEvent objects.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<CalendarEvent>> MapAllCalendarEventsFromResponse();
    }
}
