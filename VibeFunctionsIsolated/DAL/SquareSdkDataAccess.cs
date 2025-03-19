﻿using Microsoft.Extensions.Logging;
using Square;
using Square.Authentication;
using Square.Exceptions;
using Square.Models;
using VibeFunctionsIsolated.DAL.Interfaces;
using VibeFunctionsIsolated.Models.Square;
using static VibeFunctionsIsolated.Enums.SquareEnums;

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
        List<string> categoryIds = [categoryInfo.Id];

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
            logger.LogError("Exception: {message} Response Code: {responseCode}", message, responseCode);

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

        if (response?.Errors != null)
        {
            logger.LogError($"{nameof(SearchCategoryObjectsByParentId)} returned null");
        }

        return response;
    }

    public async Task<RetrieveCatalogObjectResponse?> GetCatalogObjectById(CatalogInformation categoryId)
    {
        RetrieveCatalogObjectResponse? response = null;

        try
        {
            response = await squareClient.CatalogApi.RetrieveCatalogObjectAsync(objectId: categoryId.Id, includeRelatedObjects: true);
        }
        catch (ApiException e)
        {
            Console.WriteLine("Failed to make the request");
            Console.WriteLine($"Response Code: {e.ResponseCode}");
            Console.WriteLine($"Exception: {e.Message}");
        }

        return response;
    }

    public async Task<string> GetImageURL(string? itemId)
    {
        if (itemId == null || itemId == "")
            return "";

        string imageUrl;
        RetrieveCatalogObjectResponse? item;

        try
        {
            item = await squareClient.CatalogApi.RetrieveCatalogObjectAsync(objectId: itemId);
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

    public async Task<SearchTeamMembersResponse> GetAllActiveTeamMembersAsync()
    {
        SearchTeamMembersResponse result;
        var filter = new SearchTeamMembersFilter.Builder()
            .Status("ACTIVE")
            .Build();

        var query = new SearchTeamMembersQuery.Builder()
            .Filter(filter)
            .Build();

        var body = new SearchTeamMembersRequest.Builder()
            .Query(query)
            .Build();

        try
        {
            result = await squareClient.TeamApi.SearchTeamMembersAsync(body: body);
        }
        catch (ApiException e)
        {
            logger.LogError("Exception: {message} Response Code: {responseCode}", e.Message, e.ResponseCode);
            Console.WriteLine("Failed to make the request");
            Console.WriteLine($"Exception: {e.Message}");
            result = new SearchTeamMembersResponse();
        }

        return result;
    }

    public async Task<BulkRetrieveTeamMemberBookingProfilesResponse> GetTeamMemberBookingInformation(List<string> ids)
    {
        var body = new BulkRetrieveTeamMemberBookingProfilesRequest.Builder(teamMemberIds: ids)
            .Build();
        BulkRetrieveTeamMemberBookingProfilesResponse response;

        try
        {
            response = await squareClient.BookingsApi.BulkRetrieveTeamMemberBookingProfilesAsync(body: body);
        }
        catch (ApiException e)
        {
            logger.LogError("Exception: {message} Response Code: {responseCode}", e.Message, e.ResponseCode);
            string requestName = nameof(BulkRetrieveTeamMemberBookingProfilesResponse);
            Console.WriteLine(string.Format("Failed to make the {0} request", requestName));
            
            response = new BulkRetrieveTeamMemberBookingProfilesResponse();
        }

        return response;
    }
}
