using Square.Models;
using VibeFunctionsIsolated.Models;

namespace VibeFunctionsIsolated.DAL;

public interface ISquareDAL
{
    /// <summary>
    /// Search for catalog items using the Square Catalog API  
    /// </summary>
    /// <param name="requestBody">Request body containing api prameters</param>
    /// <returns>Search catalog API response</returns>
    public Task<SearchCatalogItemsResponse?> SearchCatalogItems(SearchCatalogItemsRequest requestBody);
    public Task<SearchCatalogObjectsResponse?> SearchCatalogObjects(SearchCatalogObjectsRequest requestBody);
    public Task<SearchCatalogObjectsResponse?> SearchCategoryObjectsByParentId(ItemId categoryName);
    public Task<SearchCatalogItemsResponse?> SearchCatalogItemsByCategoryId(ItemId categoryId);
    public Task<RetrieveCatalogObjectResponse?> GetCatalogObjectById(ItemId categoryId);


    public Task<string?> GetImageURL(string? imageId);
}
