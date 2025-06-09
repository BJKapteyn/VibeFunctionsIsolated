using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VibeFunctionsIsolated.DAL.Interfaces;
using VibeFunctionsIsolated.Models.Cosmos;
using VibeFunctionsIsolated.Utility.UtilityInterfaces;

namespace VibeFunctionsIsolated.Utility
{
    public class CosmosUtility : ICosmosUtility
    {
        private readonly ICosmosDataAccess cosmosDataAccess;
        public CosmosUtility(ICosmosDataAccess cosmosDataAccess)
        {
            this.cosmosDataAccess = cosmosDataAccess;
        }

        public async Task<IEnumerable<CalendarEvent>> MapAllCalendarEventsFromResponse()
        {

        }
    }
}
