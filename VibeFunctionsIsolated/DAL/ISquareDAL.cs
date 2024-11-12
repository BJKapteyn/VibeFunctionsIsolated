using Square.Models;
using VibeFunctionsIsolated.Models;
namespace VibeFunctionsIsolated.DAL;

internal interface ISquareDAL
{
    public Task<SearchCatalogItemsResponse?> SearchCatalogItems(SearchCatalogItemsRequest requestBody);
    public Task<SearchCatalogObjectsResponse?> SearchCatalogObjects(SearchCatalogObjectsRequest requestBody);
    public Task<SearchCatalogObjectsResponse?> SearchCategoryObjectsByParentId(CategoryId categoryName);
    public Task<SearchCatalogItemsResponse?> SearchCatalogItemsByCategoryId(CategoryId categoryId);

    public Task<string?> GetImageURL(string? imageId);
}
