using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using VibeFunctionsIsolated.Models.Cosmos;
using VibeFunctionsIsolated.Utility.UtilityInterfaces;

namespace VibeFunctionsIsolated.Functions.CalendarEvents;

public class UpsertCalendarEvent(
        ILogger<UpsertCalendarEvent> logger,
        IApplicationUtility applicationUtility,
        ICalendarEventUtility cosmosCalendarEventUtility,
        ISquareUtility squareUtility) 
{
    private readonly ILogger<UpsertCalendarEvent> logger = logger;
    private readonly IApplicationUtility applicationUtility = applicationUtility;
    private readonly ICalendarEventUtility cosmosCalendarEventUtility = cosmosCalendarEventUtility;
    private readonly ISquareUtility squareUtility = squareUtility;

    [Function("UpsertCalendarEvent")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        CalendarEvent? calendarEvent = await applicationUtility.DeserializeStream<CalendarEvent>(req.Body);

        if (calendarEvent == null)
        {
            string upsertEventClass = nameof(UpsertCalendarEvent);
            logger.LogError("{upsertEventClass}: Invalid request body", upsertEventClass);

            return new BadRequestObjectResult("Invalid request body");
        }

        string? squareUpsertId = await squareUtility.UpsertCalendarEvent(calendarEvent);

        if (squareUpsertId != null)
        {

            if(calendarEvent.SquareEventId != squareUpsertId)
                calendarEvent.SquareEventId = squareUpsertId;

            bool didCosomosUpsert = await cosmosCalendarEventUtility.UpsertCalendarEvent(calendarEvent);

            if (didCosomosUpsert)
                return new OkResult();
        }


        //if (didCosomosUpsert)
        //{
        //    return new OkResult();
        //}

        return new BadRequestResult();
    }
}
