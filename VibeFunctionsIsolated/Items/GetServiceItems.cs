using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Square;
using Square.Exceptions;
using Square.Models;
using System.Text.Json;
using VibeCollectiveFunctions.Models;
using VibeCollectiveFunctions.Utility;
using VibeFunctionsIsolated.Enums;
using VibeFunctionsIsolated.Models;
using static VibeCollectiveFunctions.Enums.SquareEnums;

namespace VibeCollectiveFunctions.Items;

// Get all service offerings bundled by category
internal class GetServiceItems
{
    private readonly ILogger<GetItems> logger;
    private readonly ISquareUtility SquareUtility;

    public GetServiceItems(ILogger<GetItems> _logger, ISquareUtility squareUtility)
    {
        SquareUtility = squareUtility;
        logger = _logger;
    }

    [Function("GetServiceItems")]
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

        IEnumerable<SquareItem> squareItems = MapSquareItems(response, client, CatalogObjectType.ITEM.ToString());
        List<string?> distinctItemCategoryIds = squareItems.Select(item => item.ReportingCategoryId).Distinct().ToList();
        distinctItemCategoryIds.RemoveAll(categoryId => categoryId == null);
        IEnumerable<SquareServiceBundle> servicesByCategory = distinctItemCategoryIds.Select(categoryId =>
        {
            IEnumerable<SquareItem> itemsByCategory = squareItems.Where(item => item.ReportingCategoryId == categoryId);
            CatalogCategory category = response.Objects.Where(category => category.Id == categoryId).First().CategoryData;
            string categoryImageId = category.ImageIds?.First() ?? string.Empty;
            string categoryImageURL = SquareUtility.GetImageURL(categoryImageId, client, logger);
            SquareItem squareCategory = new SquareItem(category, categoryImageURL);

            return new SquareServiceBundle(squareCategory, itemsByCategory);
        });


        return new OkObjectResult(servicesByCategory);
    }


    private IEnumerable<SquareItem> MapSquareItems(SearchCatalogObjectsResponse response, SquareClient client, string type)
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
                                     string.Empty;
                    string imageURL = string.Empty;

                    if (imageId != string.Empty)
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
