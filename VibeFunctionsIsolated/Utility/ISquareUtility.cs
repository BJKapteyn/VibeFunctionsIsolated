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
        /// <param name="body">Request body as a stream</param>
        /// <returns>Deserialized object</returns>
        public Task<T?> DeserializeStream<T>(Stream body);

        /// <summary>
        /// Maps square catalog object api response into a collection of square items
        /// </summary>
        /// <param name="response">Square Catalog Object Response</param>
        /// <param name="catalogObjectType">Type of object to return, usually ITEM or Category</param>
        /// <returns></returns>
        public IEnumerable<SquareItem>? MapSquareProductItems(SearchCatalogObjectsResponse response, string catalogObjectType);
        public IEnumerable<SquareItem> GetItemsWithReportingCategoryId(IEnumerable<SquareItem> items, string? reportingCategoryId);
        public ISquareCatalogItem? GetItemFromCatalogObjectResponse(RetrieveCatalogObjectResponse? response);
    }
}
