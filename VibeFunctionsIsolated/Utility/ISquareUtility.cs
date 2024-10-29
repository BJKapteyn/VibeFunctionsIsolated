using Microsoft.Extensions.Logging;
using Square;

namespace VibeCollectiveFunctions.Utility
{
    internal interface ISquareUtility
    {
        public SquareClient InitializeClient();
        public string GetImageURL(string? imageId, SquareClient client, ILogger logger);
    }
}
