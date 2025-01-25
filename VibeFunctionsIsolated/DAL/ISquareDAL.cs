using Square.Models;
using VibeFunctionsIsolated.Models;
namespace VibeFunctionsIsolated.DAL;

public interface ISquareDAL
{
    public Task<SearchCatalogItemsResponse?> SearchCatalogItems(SearchCatalogItemsRequest requestBody);
    public Task<SearchCatalogObjectsResponse?> SearchCatalogObjects(SearchCatalogObjectsRequest requestBody);
    public Task<SearchCatalogObjectsResponse?> SearchCategoryObjectsByParentId(CatalogInformation categoryName);
    public Task<SearchCatalogItemsResponse?> SearchCatalogItemsByCategoryId(CatalogInformation categoryId);
    /// <summary>
    /// Calls the square api directly to get all items (doesn't use SDK)
    /// </summary>
    /// <returns>Square Item </returns>
    public Task<string> GetItemsByIdRawData(CatalogInformation catalogInfo);

    public Task<string?> GetImageURL(string? imageId);
}
