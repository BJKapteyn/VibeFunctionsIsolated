using VibeFunctionsIsolated.Models.Cosmos;

namespace VibeFunctionsIsolated.Utility.UtilityInterfaces;

/// <summary>
/// Business logic layer for blog post crud operations
/// </summary>
public interface IBlogPostUtility
{
    public Task<IEnumerable<BlogPost>> GetAllBlogPosts();
    public Task<bool> UpsertBlogPost(BlogPost blogPost);
    public Task DeleteBlogPost(string id, string partitionKey);
}
