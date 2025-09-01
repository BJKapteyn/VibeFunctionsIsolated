using Square.Models;
using VibeFunctionsIsolated.Models.Square;

namespace VibeFunctionsIsolated.DAL.Interfaces;

public interface ISquareSdkDataAccess
{
    /// <summary>
    /// Searches the catalog for items that match the specified criteria;
    /// </summary>
    /// <param name="requestBody">The request containing the search criteria</param>
    /// <returns>The task result contains a <see cref="SearchCatalogItemsResponse"/> object  with the search results, or <see langword="null"/> if no items match
    /// the criteria.</returns>
    public Task<SearchCatalogItemsResponse?> SearchCatalogItems(SearchCatalogItemsRequest requestBody);
    public Task<SearchCatalogObjectsResponse?> SearchCatalogObjects(SearchCatalogObjectsRequest requestBody);
    public Task<SearchCatalogObjectsResponse?> SearchCategoryObjectsByParentId(CatalogInformation categoryName);
    public Task<SearchCatalogItemsResponse?> SearchCatalogItemsByCategoryId(CatalogInformation categoryId);
    public Task<RetrieveCatalogObjectResponse?> GetCatalogObjectById(CatalogInformation categoryId);

    /// <summary>
    /// Get all currently active team members
    /// </summary>
    /// <returns>Square Item </returns>
    public Task<IEnumerable<TeamMemberBookingProfile>> GetAllTeamMembers();

    /// <summary>
    /// Get's image URL using the item Id from the Square API
    /// </summary>
    /// <param name="itemId">id for the item of desired image URL</param>
    /// <returns>Image URL associated with the item id</returns>
    public Task<string> GetImageURL(string? itemId);

    /// <summary>
    /// Updates or inserts a catalog object into square
    /// </summary>
    /// <param name="upsertObjectRequest">request containing catalog item to upsert</param>
    /// <returns>true if object was upserted, false if not</returns>
    public Task<UpsertCatalogObjectResponse> UpsertSquareCatalogObject(UpsertCatalogObjectRequest upsertObjectRequest);
}
