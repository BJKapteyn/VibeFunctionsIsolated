using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using Square.Models;
using VibeFunctionsIsolated.DAL.Interfaces;
using VibeFunctionsIsolated.Enums;
using VibeFunctionsIsolated.Models.Cosmos;
using VibeFunctionsIsolated.Models.Interfaces;
using VibeFunctionsIsolated.Models.Square;
using VibeFunctionsIsolated.Utility.UtilityInterfaces;
using static VibeFunctionsIsolated.Enums.SquareEnums;

namespace VibeFunctionsIsolated.Utility;

public class SquareDalUtility : ISquareUtility
{
    private readonly ILogger<SquareDalUtility> logger;
    private readonly ISquareSdkDataAccess squareSdkDal;
    private readonly ISquareApiDataAccess squareApiDal;

    public SquareDalUtility(ILogger<SquareDalUtility> logger, ISquareSdkDataAccess squareSdkDal, ISquareApiDataAccess squareApiDal) 
    {
        this.logger = logger;
        this.squareSdkDal = squareSdkDal;
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

            // Add a delay to prevent rate limiting
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

    public async Task<IEnumerable<SquareTeamMember>> MapAllBookableTeamMembers()
    {
        IEnumerable<TeamMemberBookingProfile> teamMembers = await squareSdkDal.GetAllTeamMembers();

        if (teamMembers.Any())
        {
            IEnumerable<SquareTeamMember> squareTeamMembers = teamMembers
                .Where(teamMember => teamMember.IsBookable == true)
                .Select(teamMember =>
            {
                SquareTeamMember mappedTeamMember = new (teamMember.TeamMemberId,
                    teamMember.DisplayName, 
                    teamMember.Description, 
                    teamMember.ProfileImageUrl);

                return mappedTeamMember;
            });

            return squareTeamMembers;
        }

        return [];
    }

    public async Task<CatalogItemIds?> UpsertCalendarEvent(CalendarEvent calendarEvent)
    {
        CatalogObject catalogObjectToUpsert = buildCatalogObject(calendarEvent);
        string idempotencyKey = Guid.NewGuid().ToString();
        var upsertRequest = new UpsertCatalogObjectRequest(idempotencyKey, catalogObjectToUpsert);

        CatalogItemIds? upsertCatalogObjectIds = await UpsertCatalogObject(upsertRequest, nameof(UpsertCalendarEvent));

        return upsertCatalogObjectIds;
    }

    public async Task<bool> DeleteSquareEventById(string eventId)
    {
        bool didDeleteEvent = await squareSdkDal.DeleteSquareCatalogObject(eventId);

        return didDeleteEvent;
    }

    private async Task<CatalogItemIds?> UpsertCatalogObject(UpsertCatalogObjectRequest upsertRequest, string? nameOfMethodCall = null)
    {
        CatalogItemIds? upsertIds = null;

        UpsertCatalogObjectResponse upsertResponse = await squareSdkDal.UpsertSquareCatalogObject(upsertRequest);

        if (upsertResponse.Errors?.Count > 0)
        {
            string methodCall = nameOfMethodCall ?? nameof(UpsertCatalogObject);
            logger.LogError("{methodCall}: Failed to upsert catalog object to Square", methodCall);
        }
        else
        {
            logger.LogInformation("Upserted catalog object to Square with id: {id}", upsertResponse.CatalogObject.Id);
            upsertIds = new CatalogItemIds
            {
                ItemId = upsertResponse.CatalogObject.Id,
                VariationIds = upsertResponse.CatalogObject.ItemData.Variations?.Select(variation => variation.Id).ToList() ?? new List<string>(),
                DatabaseVersion = upsertResponse.CatalogObject.Version ?? 0
            };
        }

        return upsertIds;
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

    private static CatalogObject buildCatalogObject(CalendarEvent calendarEvent)
    {
        //DateTime endDate = calendarEvent.EndDate ?? calendarEvent.StartDate.AddHours(1);
        //int eventDurationInMinutes = (int)(endDate - calendarEvent.StartDate).TotalMinutes;

        //if(eventDurationInMinutes <= 0)
        //{
        //    eventDurationInMinutes = 60;
        //}

        // needs to have an assigned team member
        // Build the item variation needed for the item data
        var itemVariation = new CatalogObject(
            type: "ITEM_VARIATION",
            id: calendarEvent.SquareVariationId ?? "#variation",
            version: calendarEvent.SquareEventVersion,
            itemData: null,
            itemVariationData: new CatalogItemVariation(
                availableForBooking: true,
                itemId: calendarEvent.SquareEventId,
                name: "Default",
                ordinal: 0,
                pricingType: "FIXED_PRICING",
                serviceDuration: 3600000,
                priceMoney: new Money((long)calendarEvent.PriceInUSD, "USD")
            )
        );
        var catelogReportingCategory = new CatalogObjectCategory(
            id: calendarEvent.SquareCalendarEventCategoryId
        );
        // Build the item data
        var itemData = new CatalogItem(
            availableOnline: true,
            name: calendarEvent.EventName,
            description: calendarEvent.EventDescription ?? "",
            reportingCategory: catelogReportingCategory,
            availableForPickup: false,
            availableElectronically: true,
            categoryId: calendarEvent.SquareCalendarEventCategoryId,
            variations: [itemVariation],
            productType: SquareProductType.AppointmentsService,
            skipModifierScreen: false,
            isTaxable: false
        );

        // Build the main catalog object
        var catalogObject = new CatalogObject(
            type: "ITEM",
            id: calendarEvent.SquareEventId,
            itemData: itemData,
            version: calendarEvent.SquareEventVersion
        );

        return catalogObject;
    }
}
