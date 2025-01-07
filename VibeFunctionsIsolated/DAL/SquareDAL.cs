using Microsoft.Extensions.Logging;
using Square;
using Square.Authentication;
using Square.Exceptions;
using Square.Models;
using System.Web;
using VibeFunctionsIsolated.Models;
using static VibeCollectiveFunctions.Enums.SquareEnums;

namespace VibeFunctionsIsolated.DAL;

public class SquareDAL : ISquareDAL
{
    #region PrivateMembers

    private readonly ILogger<SquareDAL> logger;
    private SquareClient SquareClient { get; }
    #endregion

    public SquareDAL(ILogger<SquareDAL> logger)
    {
        this.logger = logger;    
        SquareClient = InitializeClient();
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
            response = await SquareClient.CatalogApi.SearchCatalogItemsAsync(body: requestBody);
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
        List<string> categoryIds =
        [
            categoryId.Id
        ];
        
        SearchCatalogItemsRequest.Builder bodyBuilder = new SearchCatalogItemsRequest.Builder()
          .CategoryIds(categoryIds);


        if(categoryId.ProductType != null)
        {
            List<string> productTypes =
            [
                categoryId.ProductType
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
            response = await SquareClient.CatalogApi.SearchCatalogObjectsAsync(body: requestBody);
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

    public async Task<SearchCatalogObjectsResponse?> SearchCategoryObjectsByParentId(CategoryId categoryId)
    {
        List<string> objectTypes =
        [
            CatalogObjectType.CATEGORY.ToString(),
        ];

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
        RetrieveCatalogObjectResponse? item;

        try
        {
            item = await SquareClient.CatalogApi.RetrieveCatalogObjectAsync(imageId);
        }
        catch (Exception ex)
        {
            string message = ex.Message;
            logger.LogError("{message}", message);
            return null;
        }
        result = ((RetrieveCatalogObjectResponse?)null)?.MObject?.ImageData?.Url;

        return result;
    }
}
