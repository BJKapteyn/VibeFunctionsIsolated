using Microsoft.Extensions.Logging;
using Square;

namespace VibeCollectiveFunctions.Utility
{
    internal interface ISquareUtility
    {
        public Task<T?> DeserializeStream<T>(Stream body);
        public SquareClient InitializeClient();
        public string GetImageURL(string? imageId, SquareClient client, ILogger logger);
    }
}
