using Square.Models;
using System.Text.Json;
using VibeFunctionsIsolated.DAL.Interfaces;
using VibeFunctionsIsolated.Enums;
using VibeFunctionsIsolated.Models.Interfaces;
using VibeFunctionsIsolated.Models.Square;
using VibeFunctionsIsolated.Utility.UtilityInterfaces;
using static VibeFunctionsIsolated.Enums.SquareEnums;

namespace VibeFunctionsIsolated.Utility;

public class SquareDalUtility : ISquareUtility
{
    private readonly ISquareSdkDataAccess squareSdkDal;
    private readonly ISquareApiDataAccess squareApiDal;
    public SquareDalUtility(ISquareSdkDataAccess squareDAL, ISquareApiDataAccess squareApiDal) 
    {
        this.squareSdkDal = squareDAL;
        this.squareApiDal = squareApiDal;
    }

    public IEnumerable<SquareItem> MapSquareProductItems(SearchCatalogObjectsResponse response, string type)
    {
        IEnumerable<SquareItem> mappedSquareItems;

        string employeeCategoryId = response.Objects.Where(responseItem =>
        {
            return responseItem.CategoryData?.Name.Equals(Categories.Employee.ToString()) ?? false;
        })
        .First().Id;

        if (response.Objects.Count > 0)
        {
            IEnumerable<CatalogObject> squareObjects = response.Objects
            .Where(responseItem =>
            {
                bool isCorrectType = responseItem.Type == type;
                bool isNOTEmployee = responseItem.ItemData?.ReportingCategory?.Id != employeeCategoryId;
                bool isAppointment = responseItem.ItemData?.ProductType == SquareProductType.AppointmentsService;

                return isCorrectType && isNOTEmployee && isAppointment;

            }).ToList();

            mappedSquareItems = MapCatalogObjectsToLocalModel(squareObjects).Result;
        }
        else
        {
            mappedSquareItems = [];
        }

        return mappedSquareItems;
    }

    public IEnumerable<SquareItem> GetItemsByReportingCategoryId(IEnumerable<SquareItem> items, string? reportingCategoryId)
    {
        if (reportingCategoryId == null || reportingCategoryId == "")
            return items;

        IEnumerable<SquareItem> itemsWithReportingCategoryId = items.Where(item => item.ReportingCategoryId == reportingCategoryId);

        return itemsWithReportingCategoryId;
    }

    public async Task<IEnumerable<SquareItem>> MapCatalogObjectsToLocalModel(IEnumerable<CatalogObject> catalogObjects, bool needsBuyNowLinks = false)
    {
        Dictionary<string, Task<string>[]> itemIdToExtraItemProperties = [];

        IEnumerable<SquareItem> squareItems = catalogObjects.Select(responseItem =>
        {
            string imageId = responseItem.ItemData.ImageIds == null ? "" : responseItem.ItemData.ImageIds[0];
            Task<string>[] getPropertiesTasks = new Task<string>[2];

            Thread.Sleep(50);
            getPropertiesTasks[0] = squareSdkDal.GetImageURL(imageId);
            getPropertiesTasks[1] = needsBuyNowLinks ? squareApiDal.GetBuyNowLink(responseItem.Id) : new Task<string>(() => "");
            if (getPropertiesTasks[1].Status == TaskStatus.Created)
                getPropertiesTasks[1].Start();

            itemIdToExtraItemProperties.TryAdd(responseItem.Id, getPropertiesTasks);

            return new SquareItem(responseItem, "");
        }).ToList();

        // Add extra properties that needed seperate requests to the items
        foreach (SquareItem item in squareItems)
        {
            Task<string>[] extraDataTasks = itemIdToExtraItemProperties[item.Id];
            await Task.WhenAll(extraDataTasks);

            item.ImageURL = extraDataTasks[0].Result;

            if(needsBuyNowLinks)
                item.BuyNowLink = extraDataTasks[1].Result;
        }

        return squareItems;
    }

    public ISquareCatalogItem? MapItemFromCatalogObjectResponse(RetrieveCatalogObjectResponse? response)
    {
        if (response?.MObject == null)
        {
            return null;
        }

        ISquareCatalogItem? catalogItem = null;
        bool isCategory = response.MObject.CategoryData != null;
        bool isProduct = response.MObject.ItemData != null;

        if (isCategory)
        {
            catalogItem = new SquareCategory(response.MObject, null);
        }
        else if (isProduct)
        {
            catalogItem = new SquareItem(response.MObject, null);
        }

        if (catalogItem == null)
        {
            return null;
        }

        string imageUrl = findImageUrlFromCatalogObjectResponse(response);
        catalogItem.ImageURL = imageUrl;

        return catalogItem;
    }

    public async Task<IEnumerable<SquareTeamMember>> GetAllTeamMembersWithDetails()
    {
        SearchTeamMembersResponse teamMembers = await squareSdkDal.GetAllActiveTeamMembersAsync(); 

        return [];
    }

    /// <summary>
    /// Search for the image url in the response, it isn't in the same spot for all item types
    /// </summary>
    /// <param name="response">Catolog API response object</param>
    /// <returns>Image URL if found, empty string if not</returns>
    private static string findImageUrlFromCatalogObjectResponse(RetrieveCatalogObjectResponse response)
    {
        // Check main object
        string? imageUrl = response?.MObject?.ImageData?.Url;

        // Check the related objects
        imageUrl ??= response?.RelatedObjects
                        ?.Where(x => x.ImageData != null)
                        ?.FirstOrDefault()
                        ?.ImageData
                        ?.Url;

        return imageUrl ?? "";
    }
}
