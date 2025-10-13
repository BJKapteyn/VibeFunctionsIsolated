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
    private readonly IApplicationUtility applicationUtility;
    private readonly IBlogPostUtility blogPostUtility;

    public DeleteBlogPostById(
        ILogger<DeleteBlogPostById> logger,
        IBlogPostUtility blogPostUtility,
        IApplicationUtility applicationUtility)
    {
        this.logger = logger;
        this.blogPostUtility = blogPostUtility;
        this.applicationUtility = applicationUtility;
    }

    [Function("DeleteBlogPostById")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        CosmosItemId? itemToDelete = await applicationUtility.DeserializeStream<CosmosItemId>(req.Body);

        if (itemToDelete == null || string.IsNullOrEmpty(itemToDelete.id) || string.IsNullOrEmpty(itemToDelete.partitionKey))
        {
            logger.LogError("Incorrect format when running: " + nameof(DeleteBlogPostById));
            return new NotFoundObjectResult("Invalid request body");
        }

        bool didDeleteBlogPost = await blogPostUtility.DeleteBlogPost(itemToDelete.id, itemToDelete.partitionKey);

        if(!didDeleteBlogPost)
        {
            return new BadRequestObjectResult("Blog Post not deleted");
        }
        logger.LogInformation("Function {CalendarEventId} deleted", itemToDelete.id);

        return new OkObjectResult("Item Deleted Successfully");
    }
}