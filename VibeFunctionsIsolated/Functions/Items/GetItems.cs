using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Square;
using Square.Exceptions;
using Square.Models;
using VibeCollectiveFunctions.Models;
using VibeCollectiveFunctions.Utility;
using VibeFunctionsIsolated.Enums;
using static VibeCollectiveFunctions.Enums.SquareEnums;

namespace VibeCollectiveFunctions.Functions.Items;

internal class GetItems
{
    private readonly ILogger<GetItems> logger;
    private readonly ISquareUtility SquareUtility;

    public GetItems(ILogger<GetItems> _logger, ISquareUtility squareUtility)
    {
        SquareUtility = squareUtility;
        logger = _logger;
    }

    [Function("GetItems")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        SquareClient client = SquareUtility.InitializeClient();
        SearchCatalogObjectsResponse response;

        List<string> objectTypes = new()
        {
            CatalogObjectType.ITEM.ToString(),
            CatalogObjectType.CATEGORY.ToString(),
        };

        SearchCatalogObjectsRequest? requestBody = new SearchCatalogObjectsRequest.Builder()
            .ObjectTypes(objectTypes)
            .Build();

        try
        {
            response = await client.CatalogApi.SearchCatalogObjectsAsync(requestBody);
        }
        catch (ApiException e)
        {
            logger.LogError(e.Message);
            Console.WriteLine($"Response Code: {e.ResponseCode}");
            Console.WriteLine($"Exception: {e.Message}");

            return new NotFoundResult();
        }

        List<SquareItem> squareItems = MapSquareItems(response, client, CatalogObjectType.ITEM.ToString()); 

        return new OkObjectResult(squareItems); 
    }

    private List<SquareItem> MapSquareItems(SearchCatalogObjectsResponse response, SquareClient client, string type)
    {
        List<SquareItem> squareItems = new List<SquareItem>();

        string employeeCategoryId = response.Objects.Where(responseItem =>
        {
            return responseItem.CategoryData?.Name.Equals(Categories.Employee.ToString()) ?? false;
        })
        .First().Id;
        
        if (response.Objects.Count > 0)
        {
            squareItems = response.Objects
                .Where(responseItem =>
                {
                    bool isCorrectType = responseItem.Type == type;
                    bool isNOTEmployee = responseItem.ItemData?.ReportingCategory?.Id != employeeCategoryId;
                    bool isAppointment = responseItem.ItemData?.ProductType == SquareProductType.AppointmentsService;

                    return isCorrectType && isNOTEmployee && isAppointment;

                })
                .Select(responseItem =>
                {
                    string imageId = responseItem.ItemData.ImageIds != null ? 
                                     responseItem.ItemData.ImageIds.First() : 
                                     "";
                    string imageURL = "";

                    if(imageId != string.Empty)
                    {
                        imageURL = SquareUtility.GetImageURL(imageId, client, logger);
                    }

                    return new SquareItem(responseItem, imageURL);
                })
                .ToList();
        }

        return squareItems;
    }
} 
