using Square.Models;
using System.Text.Json;
using VibeFunctionsIsolated.DAL.Interfaces;
using VibeFunctionsIsolated.Enums;
using VibeFunctionsIsolated.Models;
using VibeFunctionsIsolated.Models.Interfaces;
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

    public async Task<T?> DeserializeStream<T>(Stream body)
    {
        T? deserializedJson;
        try
        {
            using (StreamReader reader = new(body))
            {
                string streamText = await reader.ReadToEndAsync();
                deserializedJson = JsonSerializer.Deserialize<T>(streamText);
            };
        }
        catch
        {
            deserializedJson = default;
        }
        return deserializedJson;
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
            mappedSquareItems = Array.Empty<SquareItem>();
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
        Dictionary<string, List<Task<string>>> itemIdToExtraData = [];

        IEnumerable<SquareItem> squareItems = catalogObjects.Select(responseItem =>
        {
            string imageId = responseItem.ItemData.ImageIds == null ? "" : responseItem.ItemData.ImageIds[0];
            List<Task<string>> getPropertiesTasks = new List<Task<string>>();

            getPropertiesTasks.Add(squareSdkDal.GetImageURL(imageId));
            if(needsBuyNowLinks)
                getPropertiesTasks.Add(squareApiDal.GetBuyNowLink(responseItem.Id));

            itemIdToExtraData.TryAdd(responseItem.Id, getPropertiesTasks);

            return new SquareItem(responseItem, "");
        }).ToList();

        await Task.WhenAll(itemIdToExtraData.Values.SelectMany(x => x).ToArray());

        // Add extra properties that needed seperate requests to the items
        foreach (SquareItem item in squareItems)
        {
            List<Task<string>> extraDataTasks = itemIdToExtraData[item.Id];

            item.ImageURL = extraDataTasks[0].Result;

            if(needsBuyNowLinks)
                item.BuyNowLink = extraDataTasks[1].Result;
        }

        return squareItems;
    }
    public ISquareCatalogItem? GetItemFromCatalogObjectResponse(RetrieveCatalogObjectResponse? response)
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

    // Search for the image url in the response, it isn't in the same spot for all item types
    private string findImageUrlFromCatalogObjectResponse(RetrieveCatalogObjectResponse response)
    {
        // Check main object first
        string? imageUrl = response?.MObject?.ImageData?.Url;

        if (imageUrl == null)
        {
            // Check in the related objects
            imageUrl = response?.RelatedObjects
                           ?.Where(x => x.ImageData != null)
                           ?.FirstOrDefault()
                           ?.ImageData
                           ?.Url;
        }

        return imageUrl ?? "";
    }
}
