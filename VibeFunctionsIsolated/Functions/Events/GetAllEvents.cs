using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using VibeFunctionsIsolated.Utility;
using VibeFunctionsIsolated.Utility.UtilityInterfaces;

namespace VibeFunctionsIsolated.Functions.Events;

public class GetAllEvents
{
    private readonly ILogger<GetAllEvents> logger;

    private readonly ICosmosCalendarEventUtility cosmosUtility;

    public GetAllEvents(ILogger<GetAllEvents> logger, ICosmosCalendarEventUtility cosmosUtility)
    {
        this.logger = logger;
        this.cosmosUtility = cosmosUtility;
    }

    [Function("GetAllEvents")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("Welcome to Azure Functions!");
    }
}