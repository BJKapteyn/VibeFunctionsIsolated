using Square.Models;
using System.Text.Json;
using VibeFunctionsIsolated.DAL.Interfaces;
using VibeFunctionsIsolated.Enums;
using VibeFunctionsIsolated.Models;
using static VibeFunctionsIsolated.Enums.SquareEnums;

namespace VibeFunctionsIsolated.Utility;


public class SquareUtility : ISquareUtility
{
    private readonly ISquareSdkDataAccess squareSdkDal;
    private readonly ISquareApiDataAccess squareApiDal;
    public SquareUtility(ISquareSdkDataAccess squareDAL, ISquareApiDataAccess squareApiDal) 
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
        IEnumerable<SquareItem>? squareItems = null;

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
                                 responseItem.ItemData.ImageIds.First() : "";
                string? imageURL = null;

                if (imageId != string.Empty)
                {
                    imageURL = squareSdkDal.GetImageURL(imageId).Result;
                }

                return new SquareItem(responseItem, imageURL);
            })
            .ToList();
        }

        if(squareItems is null)
        {
            squareItems = Array.Empty<SquareItem>();
        }

        return squareItems;
    }

    public IEnumerable<SquareItem> GetItemsWithReportingCategoryId(IEnumerable<SquareItem> items, string? reportingCategoryId)
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
            List<Task<string>> getExtraDataTasks = new List<Task<string>>();

            getExtraDataTasks.Add(squareSdkDal.GetImageURL(imageId));
            if(needsBuyNowLinks)
                getExtraDataTasks.Add(squareApiDal.GetBuyNowLink(responseItem.Id));

            itemIdToExtraData.TryAdd(responseItem.Id, getExtraDataTasks);

            return new SquareItem(responseItem, "");
        }).ToList();

        await Task.WhenAll(itemIdToExtraData.Values.SelectMany(x => x).ToArray());

        foreach (SquareItem item in squareItems)
        {
            List<Task<string>> extraDataTasks = itemIdToExtraData[item.Id];
            Task.WaitAll(extraDataTasks.ToArray());
            item.ImageURL = extraDataTasks[0].Result;

            if(needsBuyNowLinks)
                item.BuyNowLink = extraDataTasks[1].Result;
        }

        return squareItems;
    }

}
