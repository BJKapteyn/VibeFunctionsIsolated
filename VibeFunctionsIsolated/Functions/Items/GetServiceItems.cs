using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Square.Models;
using VibeFunctionsIsolated.Models;
using VibeFunctionsIsolated.Utility;
using VibeFunctionsIsolated.DAL;
using VibeFunctionsIsolated.Enums;
using VibeFunctionsIsolated.Models;
using static VibeFunctionsIsolated.Enums.SquareEnums;

namespace VibeFunctionsIsolated.Functions.Items;

// Get all service offerings bundled by category
public class GetServiceItems
{
    private readonly ILogger<GetItems> logger;
    private readonly ISquareUtility squareUtility;
    private readonly ISquareDAL squareDAL;

    public GetServiceItems(ILogger<GetItems> logger, ISquareUtility squareUtility, ISquareDAL squareDAL)
    {
        this.logger = logger;
        this.squareUtility = squareUtility;
        this.squareDAL = squareDAL;
    }

    [Function("GetServiceItems")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        List<string> objectTypes = new()
        {
            CatalogObjectType.ITEM.ToString(),
            CatalogObjectType.CATEGORY.ToString(),
        };

        SearchCatalogObjectsRequest? requestBody = new SearchCatalogObjectsRequest.Builder()
            .ObjectTypes(objectTypes)
            .Build();

        SearchCatalogObjectsResponse? response = await squareDAL.SearchCatalogObjects(requestBody);

        if(response == null)
        {
            return new NotFoundResult();
        }

        IEnumerable<SquareItem> squareItems = MapSquareItems(response, CatalogObjectType.ITEM.ToString());
        List<string?> distinctItemCategoryIds = squareItems.Select(item => item.ReportingCategoryId).Distinct().ToList();

        IEnumerable<SquareServiceBundle> servicesByCategory = distinctItemCategoryIds.Where(categoryId => categoryId != null).Select(categoryId =>
        {
            IEnumerable<SquareItem> itemsByCategory = squareItems.Where(item => item.ReportingCategoryId == categoryId);
            CatalogObject category = response.Objects.First(categoryObject => categoryObject.Id == categoryId);
            string categoryImageId = category.CategoryData.ImageIds?.First() ?? string.Empty;
            string? categoryImageURL = squareDAL.GetImageURL(categoryImageId).Result;
            SquareCategory squareCategory = new SquareCategory(category, categoryImageURL);

            return new SquareServiceBundle(squareCategory, itemsByCategory);

        });

        return new OkObjectResult(servicesByCategory);
    }


    private IEnumerable<SquareItem> MapSquareItems(SearchCatalogObjectsResponse response, string type)
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

                    string? imageURL = null;

                    if (imageId != string.Empty)
                    {
                        imageURL = squareDAL.GetImageURL(imageId).Result;
                    }

                    return new SquareItem(responseItem, imageURL);
                })
                .ToList();
        }

        return squareItems;
    }

}
