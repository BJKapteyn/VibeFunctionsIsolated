using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using VibeFunctionsIsolated.Utility.UtilityInterfaces;

namespace VibeFunctionsIsolated.Functions.CalendarEvents;

public class GetCalendarEvent
{
    private readonly ILogger<GetCalendarEvent> logger;
    private readonly ICosmosCalendarEventUtility cosmosCalendarEventUtility;

    public GetCalendarEvent(
        ILogger<GetCalendarEvent> logger,
        ICosmosCalendarEventUtility cosmosCalendarEventUtility)
    {
        this.logger = logger;
        this.cosmosCalendarEventUtility = cosmosCalendarEventUtility;
    }

    [Function("GetCalendarEvent")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        return new OkObjectResult("Welcome to Azure Functions!");
    }
}