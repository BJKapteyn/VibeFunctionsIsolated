using Azure.Core;
using Azure;
using Microsoft.Extensions.Logging;
using Square;
using Square.Authentication;
using Square.Exceptions;
using Square.Models;
using System.Text.Json;
using System.Web;
using VibeFunctionsIsolated.Models;
using static VibeFunctionsIsolated.Enums.SquareEnums;
using System.Net;

namespace VibeFunctionsIsolated.DAL;

public class SquareDAL : ISquareDAL
{
    #region Private Members

    private readonly ILogger<SquareDAL> logger;
    private SquareClient squareClient { get; }
    #endregion

    public SquareDAL(ILogger<SquareDAL> logger)
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


        if(categoryInfo.ProductType != null)
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

    public async Task<IEnumerable<SquareItemRawData>> GetItemsByIdRawData(CatalogInformation catalogInfo)
    {
        IEnumerable<SquareItemRawData> itemCollection;

        string getItemEndpoint = "https://connect.squareup.com/v2/catalog/search-catalog-items";
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, getItemEndpoint);

        request.Headers.Add("Authorization", $"Bearer {System.Environment.GetEnvironmentVariable("SquareProduction")}");
        request.Headers.Add("Accept", "application/json");
        request.Content = new StringContent(JsonSerializer.Serialize(catalogInfo));

        string responseJsonString = await getJsonStringResponse(request);

        if(responseJsonString != "")
        {
            using (JsonDocument jsonBody = JsonDocument.Parse(responseJsonString))
            {
                JsonElement root = jsonBody.RootElement;
                JsonElement items = root.GetProperty("objects");
            }
        }

        //if(false)
        //{
        //    logger.LogError($"{nameof(GetItemsByIdRawData)} returned null");
        //}

        return new List<SquareItemRawData>();
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

    public async Task<string?> GetImageURL(string? imageId)
    {
        if (imageId == null)
            return "";

        string? result;
        RetrieveCatalogObjectResponse? item;

        try
        {
            item = await squareClient.CatalogApi.RetrieveCatalogObjectAsync(imageId);
        }
        catch (Exception ex)
        {
            string message = ex.Message;
            logger.LogError("{message}", message);
            return null;
        }
        result = item?.MObject?.ImageData?.Url;

        return result;
    }

    #region Private Methods
    private async Task<string> getJsonStringResponse(HttpRequestMessage request)
    {
        HttpClient client = new HttpClient();
        HttpResponseMessage response;
        string responseBodyStr;

        try
        {
            response = await client.SendAsync(request);

            if (response.StatusCode.Equals(HttpStatusCode.OK) == false)
            {
                string message = response.ReasonPhrase ?? "";
                throw new HttpRequestException(message, null, response.StatusCode);
            }

            responseBodyStr = await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            string message = ex.Message;
            logger.LogError("{message}", message);

            responseBodyStr = "";
        }

        return responseBodyStr;
    }
    #endregion
}
