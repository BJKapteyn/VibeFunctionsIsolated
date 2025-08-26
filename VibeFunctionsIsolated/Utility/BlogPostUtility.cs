using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using VibeFunctionsIsolated.DAL.Interfaces;
using VibeFunctionsIsolated.Models.Cosmos;
using VibeFunctionsIsolated.Models.DataAccess;
using VibeFunctionsIsolated.Models.Interfaces;
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

        if (blogPosts.Count() <= 0)
        {
            logger.LogError("No Blog Posts Found");
        }

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

        CosmosResponse upsertResponse = await cosmosDataAccess.UpsertCosmosItemAsync(blogPost);

        if (upsertResponse.StatusCode == System.Net.HttpStatusCode.OK)
        {
            logger.LogInformation("BlogPost with id {id} created in CosmosDB", blogPost.id);
            didUpsert = true;
        }
        else if (upsertResponse.StatusCode == System.Net.HttpStatusCode.Created)
        {
            logger.LogInformation("BlogPost with id {id} updated in CosmosDB", blogPost.id);
            didUpsert = true;
        }
        else
        {
            Type? upsertType = upsertResponse?.CosmosItem.GetType();
            logger.LogError("Error updating or creating BlogPost with type {upsertType} in CosmosDB. StatusCode: {statusCode}", upsertType, upsertResponse?.StatusCode);
        }

        return didUpsert;
    }

    public async Task<bool> DeleteBlogPost(string id, string partitionKey)
    {
        bool didDelete = false;

        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(partitionKey))
        {
            logger.LogError(nameof(DeleteBlogPost) + " called with invalid id or partitionKey");
            return false;
        }

        try
        {
            ItemResponse<ICosmosItem> response = await cosmosDataAccess.DeleteCosmosItemAsync<ICosmosItem>(id, partitionKey);

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent ||
                response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                logger.LogInformation("BlogPost with id {id} deleted from CosmosDB", id);
                didDelete = true;
            }
            else
            {
                logger.LogWarning("Failed to delete BlogPost with id {id}. StatusCode: {statusCode}", id, response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred while deleting BlogPost with id {id}", id);
        }

        return didDelete;
    }
}
