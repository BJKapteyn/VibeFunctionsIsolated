using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using Square;
using Square.Exceptions;
using Square.Models;
using VibeFunctionsIsolated.DAL.Interfaces;
using VibeFunctionsIsolated.Models;
using static VibeFunctionsIsolated.Enums.SquareEnums;
using Square.Authentication;

namespace VibeFunctionsIsolated.DAL;

public class SquareSdkDataAccess : ISquareSdkDataAccess
{
    #region Private Members

    private readonly ILogger<SquareSdkDataAccess> logger;
    private SquareClient squareClient { get; }

    #endregion

    public SquareSdkDataAccess(ILogger<SquareSdkDataAccess> logger)
    {
        this.logger = logger;    
        squareClient = InitializeClient();
    }
    public static SquareClient InitializeClient()
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

    public async Task<SearchCatalogItemsResponse?> SearchCatalogItemsByCategoryId(CatalogInformation categoryInfo)
    {
        List<string> categoryIds = [ categoryInfo.Id ];
        
        SearchCatalogItemsRequest.Builder bodyBuilder = new SearchCatalogItemsRequest.Builder()
          .CategoryIds(categoryIds);


        if (categoryInfo.ProductType != null && categoryInfo.ProductType != "")
        {
            List<string> productTypes =
            [
                categoryInfo.ProductType
            ];

            bodyBuilder.ProductTypes(productTypes);
        }

        SearchCatalogItemsRequest body = bodyBuilder.Build();

        SearchCatalogItemsResponse? response = await SearchCatalogItems(body);

        if (response == null) 
        {
            logger.LogError($"{nameof(SearchCategoryObjectsByParentId)} returned null");
        }

        return response;
    }

    public async Task<SearchCatalogObjectsResponse?> SearchCatalogObjects(SearchCatalogObjectsRequest requestBody)
    {
        SearchCatalogObjectsResponse? response;

        try
        {
            response = await squareClient.CatalogApi.SearchCatalogObjectsAsync(body: requestBody);
        }
        catch (ApiException e)
        {
            string message = e.Message;
            string responseCode = e.ResponseCode.ToString();
            logger.LogError("{message} Response Code: {responseCode}", message, responseCode);
            logger.LogError("Exception: {message}", message);

            return null;
        }

        return response;
    }
    public async Task<SearchCatalogObjectsResponse?> SearchCategoryObjectsByParentId(CatalogInformation categoryInfo)
    {
        List<string> objectTypes =
        [
            CatalogObjectType.CATEGORY.ToString(),
        ];

        CatalogQueryExact exactQuery = new CatalogQueryExact.Builder(attributeName: "parent_category", attributeValue: categoryInfo.Id)
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

    public async Task<string> GetImageURL(string? imageId)
    {
        if (imageId == null)
            return "";

        string imageUrl;

        RetrieveCatalogObjectResponse? item;

        try
        {
            item = await squareClient.CatalogApi.RetrieveCatalogObjectAsync(imageId);
        }
        catch (Exception ex)
        {
            string message = ex.Message;
            logger.LogError("{message}", message);
            return "";
        }

        imageUrl = item?.MObject?.ImageData?.Url ?? "";

        return imageUrl;
    }

    
}
