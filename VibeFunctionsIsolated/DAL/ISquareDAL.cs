using Square.Models;
namespace VibeFunctionsIsolated.DAL;

internal interface ISquareDAL
{
    public Task<SearchCatalogItemsResponse?> SearchCatalogItem(SearchCatalogItemsRequest requestBody);
    public Task<SearchCatalogObjectsResponse?> SearchCatalogObjects(SearchCatalogObjectsRequest requestBody);
    public Task<string?> GetImageURL(string? imageId);
}
