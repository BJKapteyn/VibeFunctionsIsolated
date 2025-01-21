using Square.Models;
using System.Text.Json;
using VibeFunctionsIsolated.Models;
using VibeFunctionsIsolated.DAL;
using VibeFunctionsIsolated.Enums;
using static VibeFunctionsIsolated.Enums.SquareEnums;
using VibeFunctionsIsolated.Models.Interfaces;
using Azure;

namespace VibeFunctionsIsolated.Utility
{
    public class SquareUtility : ISquareUtility
    {
        private readonly ISquareDAL squareDAL;
        public SquareUtility(ISquareDAL squareDAL) 
        {
            this.squareDAL = squareDAL;
        }

        public async Task<T?> DeserializeStream<T>(Stream body)
        {
            T? deserializedJson;
            using (StreamReader reader = new(body))
            {
                string streamText = await reader.ReadToEndAsync();
                deserializedJson = JsonSerializer.Deserialize<T>(streamText);
            };

            return deserializedJson;
        }

        public IEnumerable<SquareItem>? MapSquareProductItems(SearchCatalogObjectsResponse response, string type)
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
                                         responseItem.ItemData.ImageIds.First() :
                                         "";
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

        public IEnumerable<SquareItem> GetItemsWithReportingCategoryId(IEnumerable<SquareItem> items, string? reportingCategoryId)
        {
            IEnumerable<SquareItem> itemsWithReportingCategoryId = items.Where(item => item.ReportingCategoryId == reportingCategoryId);

            return itemsWithReportingCategoryId;
        }

        public ISquareCatalogItem? GetItemFromCatalogObjectResponse(RetrieveCatalogObjectResponse? response)
        {
            if(response?.MObject == null)
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

            if(catalogItem == null)
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
}
