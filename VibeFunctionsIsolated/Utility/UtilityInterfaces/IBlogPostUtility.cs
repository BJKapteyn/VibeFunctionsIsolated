using VibeFunctionsIsolated.Models.Cosmos;

namespace VibeFunctionsIsolated.Utility.UtilityInterfaces
{
    public interface IBlogPostUtility
    {
        public Task<IEnumerable<BlogPost>> GetAllBlogPosts();
        public Task<bool> UpsertBlogPost(BlogPost blogPost);
        
    }
}
