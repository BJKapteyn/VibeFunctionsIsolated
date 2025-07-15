using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using VibeFunctionsIsolated.Models.Cosmos;
using VibeFunctionsIsolated.Utility.UtilityInterfaces;

namespace VibeFunctionsIsolated.Functions.BlogPosts;

public class GetAllBlogPosts
{
    private readonly ILogger<GetAllBlogPosts> logger;
    private readonly IBlogPostUtility blogPostUtility;

    public GetAllBlogPosts(ILogger<GetAllBlogPosts> logger, IBlogPostUtility blogPostUtility)
    {
        this.logger = logger;
        this.blogPostUtility = blogPostUtility;
    }

    [Function("GetAllBlogPosts")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        IEnumerable<BlogPost> blogPosts = await blogPostUtility.GetAllBlogPosts();

        if (blogPosts == null || !blogPosts.Any())
        {
            logger.LogInformation("No blog posts found.");
            return new NotFoundResult();
        }
        return new OkResult();
    }
}
