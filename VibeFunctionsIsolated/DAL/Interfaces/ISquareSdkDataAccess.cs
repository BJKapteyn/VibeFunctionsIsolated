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
    public Task<IEnumerable<TeamMemberBookingProfile>> GetAllTeamMembers();

    ///// <summary>
    ///// Get all currently active team members
    ///// </summary>
    ///// <returns>currently active team members</returns>
    //public Task<SearchTeamMembersResponse> GetAllActiveTeamMembers();
    
    ///// <summary>
    ///// Get the booking information of team members by their ids
    ///// </summary>
    ///// <param name="ids">Collection of Team Member Ids</param>
    ///// <returns>booking information of team members</returns>
    //public Task<BulkRetrieveTeamMemberBookingProfilesResponse> GetTeamMemberBookingInformation(List<string> ids);

    ///// <summary>
    ///// Calls the square api directly to get all items (doesn't use SDK)
    ///// </summary>
    ///// <returns>Square Item </returns>
    //public Task<IEnumerable<SquareItemRawData>> GetSquareAPIRawData(CatalogInformation catalogInfo);

    /// <summary>
    /// Get Image Url for a catalog object from the square catalog API
    /// </summary>
    /// <param name="itemId">Id of the the object that needs an image URL</param>
    /// <returns>Image URL of the item</returns>
    public Task<string> GetImageURL(string? itemId);
}
