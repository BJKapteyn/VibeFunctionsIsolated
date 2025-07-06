using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using VibeFunctionsIsolated.Models.Cosmos;
using VibeFunctionsIsolated.Utility.UtilityInterfaces;

namespace VibeFunctionsIsolated.Functions.Events;

public class GetAllCalendarEvents
{
    private readonly ILogger<GetAllCalendarEvents> logger;

    private readonly ICosmosCalendarEventUtility cosmosUtility;

    public GetAllCalendarEvents(ILogger<GetAllCalendarEvents> logger, ICosmosCalendarEventUtility cosmosUtility)
    {
        this.logger = logger;
        this.cosmosUtility = cosmosUtility;
    }

    [Function("GetAllCalendarEvents")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        IEnumerable<CalendarEvent> calendarEvents = await cosmosUtility.GetAllCalendarEvents();

        if(calendarEvents.Any())
        {
            logger.LogInformation("C# HTTP trigger function processed a request.");

            return new OkObjectResult(calendarEvents);
        }

        return new BadRequestResult();
    }
}