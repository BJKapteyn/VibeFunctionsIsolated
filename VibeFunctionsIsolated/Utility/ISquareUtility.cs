using Microsoft.Extensions.Logging;
using Square;
using Square.Models;
using VibeCollectiveFunctions.Models;

namespace VibeCollectiveFunctions.Utility
{
    internal interface ISquareUtility
    {
        public Task<T?> DeserializeStream<T>(Stream body);
        public SquareClient InitializeClient();
        public string GetImageURL(string? imageId, SquareClient client, ILogger logger);
        public IEnumerable<SquareItem>? MapSquareItems(SearchCatalogObjectsResponse response, SquareClient client, string type);

    }
}
