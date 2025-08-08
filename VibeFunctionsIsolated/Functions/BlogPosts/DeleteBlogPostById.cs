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
    private readonly IBlogPostUtility blogPostUtility;
    private readonly IApplicationUtility applicationUtility;

    public DeleteBlogPostById(
        ILogger<DeleteBlogPostById> logger,
        IBlogPostUtility blogPostUtility,
        IApplicationUtility applicationUtility)
    {
        this.logger = logger;
        this.blogPostUtility = blogPostUtility ;
        this.applicationUtility = applicationUtility;
    }

    [Function("DeleteBlogPostById")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        CosmosItemId? itemToDelete = await applicationUtility.DeserializeStream<CosmosItemId>(req.Body);
     
        if (itemToDelete == null || itemToDelete.id.Length <= 0)
        {
            return new NotFoundObjectResult("Item to delete not found");
        }

        await blogPostUtility.DeleteBlogPost(itemToDelete.id, itemToDelete.partitionKey);
        logger.LogInformation("Function {}", itemToDelete.id);

        return new OkObjectResult("Item Deleted Successfully");
    }
}
