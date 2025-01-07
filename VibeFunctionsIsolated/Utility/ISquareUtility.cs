using Square.Models;
using VibeCollectiveFunctions.Models;

namespace VibeCollectiveFunctions.Utility
{
    /// <summary>
    /// Utility methods for square api calls
    /// </summary>
    public interface ISquareUtility
    {
        /// <summary>
        /// Deserialize into api model
        /// </summary>
        /// <typeparam name="T">The type of model to deserialize into</typeparam>
        /// <param name="body"></param>
        /// <returns>Deserialized object</returns>
        public Task<T?> DeserializeStream<T>(Stream body);
        public IEnumerable<SquareItem>? MapSquareProductItems(SearchCatalogObjectsResponse response, string type);
        public IEnumerable<SquareItem> GetItemsWithReportingCategoryId(IEnumerable<SquareItem> items, string? reportingCategoryId);
    }
}
