using Square.Models;
using VibeFunctionsIsolated.Models;
using VibeFunctionsIsolated.Models.Interfaces;

namespace VibeFunctionsIsolated.Utility
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
        /// <param name="body">Body of the response as a stream</param>
        /// <returns>Deserialized object</returns>
        public Task<T?> DeserializeStream<T>(Stream body);
        public IEnumerable<SquareItem> MapSquareProductItems(SearchCatalogObjectsResponse response, string type);
        public IEnumerable<SquareItem> GetItemsWithReportingCategoryId(IEnumerable<SquareItem> items, string? reportingCategoryId);
        public ISquareCatalogItem? GetItemFromCatalogObjectResponse(RetrieveCatalogObjectResponse? response);

        /// <summary>
        /// Add image urls to the items
        /// </summary>
        /// <param name="items">Items that need image urls</param>
        /// <param name="needsBuyNowLinks">If the items need buy now links</param>
        /// <param name="needsImageUrls">If the items need image urls</param>   
        /// <returns>The original collection with image urls</returns>
        public Task<IEnumerable<SquareItem>> MapCatalogObjectsToLocalModel(IEnumerable<CatalogObject> catalogObjects, bool needsBuyNowLinks);
    }
}
