using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using VibeFunctionsIsolated.Models.Cosmos;
using VibeFunctionsIsolated.Models.Cosmos.UtilityModels;
using VibeFunctionsIsolated.Utility.UtilityInterfaces;

namespace VibeFunctionsIsolated.Functions.BlogPosts;

public class GetBlogPostById
{
    private readonly ILogger<GetBlogPostById> _logger;
    private readonly IBlogPostUtility blogPostUtility;
    private readonly IApplicationUtility applicationUtility;

    public GetBlogPostById(
        ILogger<GetBlogPostById> logger,
        IBlogPostUtility blogPostUtility,
        IApplicationUtility applicationUtility)
    {
        _logger = logger;
        this.blogPostUtility = blogPostUtility;
        this.applicationUtility = applicationUtility;
    }

    [Function("GetBlogPostById")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        CosmosItemId? blogPostId = await applicationUtility.DeserializeStream<CosmosItemId?>(req.Body);

        if (blogPostId == null)
        {
            _logger.LogError("Blog post ID is missing in the request body for function: " + nameof(GetBlogPostById));
            return new BadRequestObjectResult("Blog post ID is required");
        }

        BlogPost? blogPost = await blogPostUtility.GetBlogPost(blogPostId.id, blogPostId.partitionKey);

        if (blogPost == null)
        {
            _logger.LogWarning("Blog post not found with ID: " + blogPostId);
            return new NotFoundObjectResult("Blog post not found");
        }

        return new OkObjectResult(blogPost);
    }
}