using Microsoft.Extensions.Logging;
using Square;
using Square.Authentication;
using Square.Exceptions;
using Square.Models;
using VibeCollectiveFunctions.Utility;
using VibeFunctionsIsolated.Functions.Items;

namespace VibeFunctionsIsolated.DAL;

internal class SquareDAL : ISquareDAL
{
    private readonly ILogger<SquareDAL> logger;
    public SquareDAL(ILogger<SquareDAL> logger)
    {
        this.logger = logger;    
        squareClient = InitializeClient();
    }
    SquareClient squareClient { get; set; }

    public SquareClient InitializeClient()
    {
        BearerAuthModel bearerAuth = new BearerAuthModel.Builder(System.Environment.GetEnvironmentVariable("SquareProduction")).Build();
        SquareClient client = new SquareClient.Builder()
            .Environment(Square.Environment.Production)
            .BearerAuthCredentials(bearerAuth)
            .Build();

        return client;
    }

    public async Task<SearchCatalogItemsResponse?> SearchCatalogItem(SearchCatalogItemsRequest requestBody)
    {
        SearchCatalogItemsResponse? response = null;

        try
        {
            response = await squareClient.CatalogApi.SearchCatalogItemsAsync(body: requestBody);
        }
        catch (ApiException e)
        {
            Console.WriteLine($"{nameof(GetProductItems)} Response Code: {e.ResponseCode}");
            Console.WriteLine($"Exception: {e.Message}");
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
            Console.WriteLine($"{nameof(GetProductItems)} Response Code: {e.ResponseCode}");
            Console.WriteLine($"Exception: {e.Message}");

            return null;
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
