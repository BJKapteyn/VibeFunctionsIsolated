using Square.Models;
using VibeFunctionsIsolated.Models.Square;
namespace VibeFunctionsIsolated.DAL.Interfaces;

public interface ISquareSdkDataAccess
{
    public Task<SearchCatalogItemsResponse?> SearchCatalogItems(SearchCatalogItemsRequest requestBody);
    public Task<SearchCatalogObjectsResponse?> SearchCatalogObjects(SearchCatalogObjectsRequest requestBody);
    public Task<SearchCatalogObjectsResponse?> SearchCategoryObjectsByParentId(CatalogInformation categoryName);
    public Task<SearchCatalogItemsResponse?> SearchCatalogItemsByCategoryId(CatalogInformation categoryId);
    public Task<RetrieveCatalogObjectResponse?> GetCatalogObjectById(CatalogInformation categoryId);

    ///// <summary>
    ///// Get all currently active team members
    ///// </summary>
    ///// <returns>currently active team members</returns>
    public Task<IEnumerable<TeamMemberBookingProfile>> GetAllTeamMembers();

    /// <summary>
    /// Get Image Url for a catalog object from the square catalog API
    /// </summary>
    /// <param name="itemId">Id of the the object that needs an image URL</param>
    /// <returns>Image URL of the item</returns>
    public Task<string> GetImageURL(string? itemId);
}
