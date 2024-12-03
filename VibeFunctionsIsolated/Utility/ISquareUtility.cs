using Square.Models;
using VibeCollectiveFunctions.Models;

namespace VibeCollectiveFunctions.Utility
{
    public interface ISquareUtility
    {
        public Task<T?> DeserializeStream<T>(Stream body);
        public IEnumerable<SquareItem>? MapSquareProductItems(SearchCatalogObjectsResponse response, string type);

    }
}
