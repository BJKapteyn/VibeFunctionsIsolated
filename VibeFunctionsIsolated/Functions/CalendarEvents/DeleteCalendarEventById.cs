using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using VibeFunctionsIsolated.Models.Cosmos.UtilityModels;
using VibeFunctionsIsolated.Utility.UtilityInterfaces;

namespace VibeFunctionsIsolated.Functions.CalendarEvents;

public class DeleteCalendarEventById
{
    private readonly ILogger<DeleteCalendarEventById> logger;
    private readonly ISquareUtility squareUtility;
    private readonly IApplicationUtility applicationUtility;
    private readonly ICalendarEventUtility calendarEventUtility;

    public DeleteCalendarEventById(
        ILogger<DeleteCalendarEventById> logger,
        IApplicationUtility applicationUtility,
        ISquareUtility squareUtility,
        ICalendarEventUtility calendarEventUtility)
    {
        this.logger = logger;
        this.applicationUtility = applicationUtility;
        this.calendarEventUtility = calendarEventUtility;
        this.squareUtility = squareUtility;
    }

    [Function("DeleteCalendarEventById")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        CosmosDeleteItemId? itemToDelete = await applicationUtility.DeserializeStream<CosmosDeleteItemId>(req.Body);
     
        if (itemToDelete == null || 
            itemToDelete.id.Length <= 0 || 
            itemToDelete.squareEventId.Length <= 0)
        {
            return new NotFoundObjectResult("Item to delete not found");
        }

        bool didDeleteCalendarEvent = await calendarEventUtility.DeleteCalendarEvent(itemToDelete.id, itemToDelete.partitionKey);


        if (didDeleteCalendarEvent)
        {
            logger.LogInformation("Calendar Event {id} deleted", itemToDelete.id);

            bool didDeleteSquareEvent = await squareUtility.DeleteSquareEventById(itemToDelete.squareEventId);

            if (didDeleteSquareEvent)
            {
                logger.LogInformation("Calendar Event {CalendarEventId} deleted", itemToDelete.id);

                return new OkObjectResult("Item Deleted Successfully");
            }
            else
            {
                logger.LogError("Failed to delete Calendar Event {CalendarEventId}", itemToDelete.id);

            }
        }

        return new BadRequestObjectResult("Failed to delete Calendar Event");
    }
}
