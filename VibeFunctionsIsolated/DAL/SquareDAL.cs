using Microsoft.Extensions.Logging;
using Square;
using Square.Authentication;
using Square.Exceptions;
using Square.Models;
using VibeFunctionsIsolated.Models;
using static VibeCollectiveFunctions.Enums.SquareEnums;

namespace VibeFunctionsIsolated.DAL;

public class SquareDAL : ISquareDAL
{
    private readonly ILogger<SquareDAL> logger;
    private SquareClient squareClient { get; }
    public SquareDAL(ILogger<SquareDAL> logger)
    {
        this.logger = logger;    
        squareClient = InitializeClient();
    }

    public SquareClient InitializeClient()
    {
        BearerAuthModel bearerAuth = new BearerAuthModel.Builder(System.Environment.GetEnvironmentVariable("SquareProduction")).Build();
        SquareClient client = new SquareClient.Builder()
            .Environment(Square.Environment.Production)
            .BearerAuthCredentials(bearerAuth)
            .Build();

        return client;
    }

    public async Task<SearchCatalogItemsResponse?> SearchCatalogItems(SearchCatalogItemsRequest requestBody)
    {
        SearchCatalogItemsResponse? response = null;

        try
        {
            response = await squareClient.CatalogApi.SearchCatalogItemsAsync(body: requestBody);
        }
        catch (ApiException e)
        {
            Console.WriteLine($"{nameof(SearchCatalogItems)} Response Code: {e.ResponseCode}");
            Console.WriteLine($"Exception: {e.Message}");
        }

        return response;
    }

    public async Task<SearchCatalogItemsResponse?> SearchCatalogItemsByCategoryId(CategoryId categoryId)
    {
        var categoryIds = new List<string>()
        {
            categoryId.Id
        };

        var body = new SearchCatalogItemsRequest.Builder()
          .CategoryIds(categoryIds)
          .Build();

        SearchCatalogItemsResponse? response = await SearchCatalogItems(body);

        if (response == null) 
        {
            logger.LogError($"{nameof(SearchCategoryObjectsByParentId)} returned null");
        }

        return response;
    }

    public async Task<SearchCatalogObjectsResponse?> SearchCatalogObjects(SearchCatalogObjectsRequest requestBody)
    {
        SearchCatalogObjectsResponse? response = null;

        try
        {
            response = await squareClient.CatalogApi.SearchCatalogObjectsAsync(body: requestBody);
        }
        catch (ApiException e)
        {
            logger.LogError($"{nameof(SearchCatalogObjects)} Response Code: {e.ResponseCode}");
            logger.LogError($"Exception: {e.Message}");

            return null;
        }

        return response;
    }

    public async Task<SearchCatalogObjectsResponse?> SearchCategoryObjectsByParentId(CategoryId categoryId)
    {
        List<string> objectTypes = new()
        {
            CatalogObjectType.CATEGORY.ToString(),
        };

        CatalogQueryExact exactQuery = new CatalogQueryExact.Builder(attributeName: "parent_category", attributeValue: categoryId.Id)
        .Build();

        var searchQuery = new CatalogQuery.Builder()
          .ExactQuery(exactQuery)
          .Build();


        SearchCatalogObjectsRequest requestBody = new SearchCatalogObjectsRequest.Builder()
            .ObjectTypes(objectTypes)
            .Query(searchQuery)
            .Build();

        SearchCatalogObjectsResponse? response = await SearchCatalogObjects(requestBody);

        if (response == null)
        {
            logger.LogError($"{nameof(SearchCategoryObjectsByParentId)} returned null");
        }

        return response;
    }

    public async Task<string?> GetImageURL(string? imageId)
    {
        if (imageId == null)
            return "";

        string? result;
        RetrieveCatalogObjectResponse? item = null;

        try
        {
            item = await squareClient.CatalogApi.RetrieveCatalogObjectAsync(imageId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return null;
        }
        result = item?.MObject?.ImageData?.Url;

        return result;
    }
}
