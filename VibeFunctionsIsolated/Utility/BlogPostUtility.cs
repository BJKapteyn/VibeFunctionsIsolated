using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System.Net;
using VibeFunctionsIsolated.DAL.Interfaces;
using VibeFunctionsIsolated.Models.Cosmos;
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

    public async Task<BlogPost?> UpsertBlogPost(BlogPost blogPost)
    {
        if (blogPost == null)
        {
            ArgumentNullException argumentNullException = new(nameof(blogPost), "Blog post cannot be null");

            throw argumentNullException;
        }

        CosmosResponse upsertResponse = await cosmosDataAccess.UpsertCosmosItemAsync(blogPost);

        if (upsertResponse.StatusCode == System.Net.HttpStatusCode.OK)
        {
            logger.LogInformation("BlogPost with id {id} created in CosmosDB", blogPost.id);
        }
        else if (upsertResponse.StatusCode == System.Net.HttpStatusCode.Created)
        {
            logger.LogInformation("BlogPost with id {id} updated in CosmosDB", blogPost.id);
        }
        else
        {
            Type? upsertType = upsertResponse?.CosmosItem.GetType();
            logger.LogError("Error updating or creating BlogPost with type {upsertType} in CosmosDB. StatusCode: {statusCode}", upsertType, upsertResponse?.StatusCode);
            
            return null;
        }
        
        blogPost.id = upsertResponse.CosmosItem.id;

        return blogPost;
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

    public async Task<BlogPost?> GetBlogPost(string id, string partitionKey)
    {
        PartitionKey partitionKeyObject = new(partitionKey);
        CosmosResponse blogPostResponse;
        BlogPost? blogPost = null;

        try
        {
            blogPostResponse =  await cosmosDataAccess.GetItemWithStatusCodeAsync<BlogPost>(id, partitionKey);

            switch (blogPostResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                    blogPost = blogPostResponse.CosmosItem as BlogPost;

                    if (blogPost == null)
                    {
                        logger.LogError("Retrieved item for id {id} could not be cast to BlogPost. ItemType: {type}", id, blogPostResponse.CosmosItem?.GetType());
                    }
                    
                    logger.LogInformation("BlogPost with id {id} retrieved successfully from CosmosDB", id);
                    break;

                case HttpStatusCode.NotFound:
                    logger.LogWarning("BlogPost with id {id} not found in CosmosDB. StatusCode: {statusCode}", id, blogPostResponse.StatusCode);
                    break;

                case HttpStatusCode.Unauthorized:
                case HttpStatusCode.Forbidden:
                    logger.LogError("Authorization error while retrieving BlogPost with id {id}. StatusCode: {statusCode}", id, blogPostResponse.StatusCode);
                    break;

                default:
                    logger.LogError("Error retrieving BlogPost with id {id}. StatusCode: {statusCode}, ItemType: {type}", id, blogPostResponse.StatusCode, blogPostResponse.CosmosItem?.GetType());
                    break;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred while retrieving BlogPost with id {id}", id);
        }

        return blogPost;
    }
}
