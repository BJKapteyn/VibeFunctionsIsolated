using Square.Models;
using VibeFunctionsIsolated.Models.Cosmos;
using VibeFunctionsIsolated.Models.Interfaces;
using VibeFunctionsIsolated.Models.Square;

namespace VibeFunctionsIsolated.Utility.UtilityInterfaces;

/// <summary>
/// Business logic layer for Square related operations
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
    /// <param name="catalogObjects">Items that need extra properties</param>
    /// <param name="needsBuyNowLinks">If the items need buy/book now links</param>
    /// <returns>The original collection with image urls</returns>
    public Task<IEnumerable<SquareItem>> MapCatalogObjectsToLocalModel(IEnumerable<CatalogObject> catalogObjects, bool needsBuyNowLinks);

    /// <summary>
    /// Get all active team members and their booking information
    /// </summary>
    /// <returns>All team members and their booking information</returns>
    public Task<IEnumerable<SquareTeamMember>> MapAllBookableTeamMembers();

    /// <summary>
    /// Upsert a calendar event to Square Catalog
    /// </summary>
    /// <param name="calendarEvent"></param>
    /// <returns></returns>
    public Task<string?> UpsertCalendarEvent(CalendarEvent calendarEvent);

}
