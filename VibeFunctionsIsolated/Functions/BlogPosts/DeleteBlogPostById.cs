using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using VibeFunctionsIsolated.Models.Cosmos.UtilityModels;
using VibeFunctionsIsolated.Utility.UtilityInterfaces;

namespace VibeFunctionsIsolated.Functions.BlogPosts;

public class DeleteBlogPostById
{
    private readonly ILogger<DeleteBlogPostById> logger;
    private readonly ICalendarEventUtility calendarEventUtility;
    private readonly IApplicationUtility applicationUtility;

    public DeleteBlogPostById(
        ILogger<DeleteBlogPostById> logger,
        ICalendarEventUtility calendarEventUtility,
        IApplicationUtility applicationUtility)
    {
        this.logger = logger;
        this.calendarEventUtility = calendarEventUtility;
        this.applicationUtility = applicationUtility;
    }

    [Function("DeleteBlogPostById")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        CosmosItemId? itemToDelete = await applicationUtility.DeserializeStream<CosmosItemId>(req.Body);

        if (itemToDelete == null)
        {
            logger.LogError("Incorrect format when running: " + nameof(DeleteBlogPostById));
            return new BadRequestObjectResult("Incorrect format");
        }

        await calendarEventUtility.DeleteCalendarEvent(itemToDelete.id, itemToDelete.partitionKey);
        logger.LogInformation("Function {CalendarEventId} deleted", itemToDelete.id);

        return new OkObjectResult("Item Deleted Successfully");
    }
}