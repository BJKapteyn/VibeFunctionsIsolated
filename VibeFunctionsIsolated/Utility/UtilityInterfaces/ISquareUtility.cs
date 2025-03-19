using Square.Models;
using VibeFunctionsIsolated.Models.Interfaces;
using VibeFunctionsIsolated.Models.Square;

namespace VibeFunctionsIsolated.Utility.UtilityInterfaces
{
    /// <summary>
    /// Utility methods for square api calls
    /// </summary>
    public interface ISquareUtility
    {

        public IEnumerable<SquareItem> MapSquareProductItems(SearchCatalogObjectsResponse response, string type);
        public IEnumerable<SquareItem> GetItemsByReportingCategoryId(IEnumerable<SquareItem> items, string? reportingCategoryId);

        /// <summary>
        /// Map a single catalog object response to a local model
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public ISquareCatalogItem? MapItemFromCatalogObjectResponse(RetrieveCatalogObjectResponse? response);

        /// <summary>
        /// Retrieve data points that aren't included in the catalog object
        /// </summary>
        /// <param name="items">Items that need extra properties</param>
        /// <param name="needsBuyNowLinks">If the items need buy/book now links</param>
        /// <param name="needsImageUrls">If the items need image urls</param>   
        /// <returns>The original collection with image urls</returns>
        public Task<IEnumerable<SquareItem>> MapCatalogObjectsToLocalModel(IEnumerable<CatalogObject> catalogObjects, bool needsBuyNowLinks);

        /// <summary>
        /// Get all active team members and their booking information
        /// </summary>
        /// <returns>All team members and their booking information</returns>
        public Task<IEnumerable<SquareTeamMember>> GetAllTeamMembersWithDetails();
    }
}
