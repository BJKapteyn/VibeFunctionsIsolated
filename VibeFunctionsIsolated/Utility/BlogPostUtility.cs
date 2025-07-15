using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using VibeFunctionsIsolated.DAL.Interfaces;
using VibeFunctionsIsolated.Models.Cosmos;
using VibeFunctionsIsolated.Utility.UtilityInterfaces;

namespace VibeFunctionsIsolated.Utility;

public class BlogPostUtility : IBlogPostUtility
{
    private readonly ICosmosDataAccess cosmosDataAccess;
    private readonly ILogger<BlogPostUtility> logger;

    public BlogPostUtility(ICosmosDataAccess cosmosDataAccess, ILogger<BlogPostUtility> logger)
    {
        this.cosmosDataAccess = cosmosDataAccess;
        this.logger = logger;

        cosmosDataAccess.ChangeContainerName("BlogPosts");
    }

    public async Task<IEnumerable<BlogPost>> GetAllBlogPosts()
    {
        const string query = "SELECT * FROM c";
        IEnumerable<BlogPost> blogPosts = await cosmosDataAccess.GetAllItemsAsync<BlogPost>(query);

        return blogPosts;
    }

    public async Task<bool> UpsertBlogPost(BlogPost blogPost)
    {
        if (blogPost == null)
        {
            ArgumentNullException argumentNullException = new(nameof(blogPost), "Blog post cannot be null");

            throw argumentNullException;
        }

        bool didUpsert = false;

        ItemResponse<BlogPost> upsertResponse = await cosmosDataAccess.UpsertCosmosItemAsync(blogPost, blogPost.id);

        if (upsertResponse.StatusCode == System.Net.HttpStatusCode.OK)
        {
            logger.LogInformation("BlogPost with id {id} updated in CosmosDB", blogPost.id);
            didUpsert = true;
        }
        else if (upsertResponse.StatusCode == System.Net.HttpStatusCode.Created)
        {
            logger.LogInformation("BlogPost with id {id} created in CosmosDB", blogPost.id);
            didUpsert = true;
        }
        else
        {
            Type? upsertType = upsertResponse?.Resource.GetType();
            logger.LogError("Error updating or creating BlogPost with type {upsertType} in CosmosDB. StatusCode: {statusCode}", upsertType, upsertResponse?.StatusCode);
        }

        return didUpsert;
    }
}
