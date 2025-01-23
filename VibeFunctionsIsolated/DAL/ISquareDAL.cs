using Square.Models;
using VibeFunctionsIsolated.Models;
namespace VibeFunctionsIsolated.DAL;

public interface ISquareDAL
{
    public Task<SearchCatalogItemsResponse?> SearchCatalogItems(SearchCatalogItemsRequest requestBody);
    public Task<SearchCatalogObjectsResponse?> SearchCatalogObjects(SearchCatalogObjectsRequest requestBody);
    public Task<SearchCatalogObjectsResponse?> SearchCategoryObjectsByParentId(CategoryInformation categoryName);
    public Task<SearchCatalogItemsResponse?> SearchCatalogItemsByCategoryId(CategoryInformation categoryId);
    /// <summary>
    /// Calls the square api directly to get all items (doesn't use SDK)
    /// </summary>
    /// <returns>Square Item </returns>
    public Task<string> GetItemsByIdRawData();

    public Task<string?> GetImageURL(string? imageId);
}
