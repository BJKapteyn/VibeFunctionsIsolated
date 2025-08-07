using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;
using VibeFunctionsIsolated.DAL.Interfaces;
using VibeFunctionsIsolated.Models.Cosmos;
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
     
        if (itemToDelete?.id.Length > 0)
        {
            await blogPostUtility.DeleteBlogPost(itemToDelete.id, itemToDelete.partitionKey);
            
            return new OkObjectResult("Item Deleted Successfully");
        }

        return new NotFoundObjectResult("Item to delete not found");
    }
}
