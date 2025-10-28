using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using VibeFunctionsIsolated.Models.Cosmos;
using VibeFunctionsIsolated.Utility.UtilityInterfaces;

namespace VibeFunctionsIsolated.Functions.BlogPosts;

public class UpsertBlogPost
{
    private readonly ILogger<UpsertBlogPost> logger;
    private readonly IBlogPostUtility blogPostUtility; 
    private readonly IApplicationUtility applicationUtility;

    public UpsertBlogPost(ILogger<UpsertBlogPost> logger, IBlogPostUtility blogPostUtility, IApplicationUtility applicationUtility)
    {
        this.logger = logger;
        this.blogPostUtility = blogPostUtility;
        this.applicationUtility = applicationUtility;
    }

    [Function("UpsertBlogPost")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        BlogPost? blogPost = null;

        blogPost = await applicationUtility.DeserializeStream<BlogPost>(req.Body);
        
        if (blogPost == null)
        {
            string upsertBlogPostClass = nameof(UpsertBlogPost);
            logger.LogError("{upsertBlogPostClass}: Invalid request body", upsertBlogPostClass);

            return new BadRequestObjectResult("Invalid request body");
        }

        bool didUpsertBlogPost = await blogPostUtility.UpsertBlogPost(blogPost);

        if (!didUpsertBlogPost)
        {
            return new BadRequestObjectResult("Upsert failed");
        }

        return new OkObjectResult(blogPost);
    }
}
