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
    ///// <returns>Square Item </returns>
    //public Task<IEnumerable<SquareItemRawData>> GetSquareAPIRawData(CatalogInformation catalogInfo);
    public Task<string> GetImageURL(string? imageId);
}
