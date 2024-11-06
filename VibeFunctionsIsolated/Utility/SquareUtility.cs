using Microsoft.Extensions.Logging;
using Square;
using Square.Authentication;
using Square.Models;
using System.Text.Json;
using static VibeCollectiveFunctions.Enums.SquareEnums;
using VibeCollectiveFunctions.Models;
using VibeFunctionsIsolated.Enums;

namespace VibeCollectiveFunctions.Utility
{
    internal class SquareUtility : ISquareUtility
    {
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

        public SquareClient InitializeClient()
        {
            BearerAuthModel bearerAuth = new BearerAuthModel.Builder(System.Environment.GetEnvironmentVariable("SquareProduction")).Build();
            SquareClient client = new SquareClient.Builder()
                .Environment(Square.Environment.Production)
                .BearerAuthCredentials(bearerAuth)
                .Build();

            return client;
        }

        public string GetImageURL(string? imageId, SquareClient client, ILogger logger)
        {
            if (imageId == null)
                return "";

            string? result;
            CatalogObject? item;

            try
            {
                item = client.CatalogApi.RetrieveCatalogObject(imageId)?.MObject;
                result = item?.ImageData?.Url ?? "";
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                result = "";
            }

            return result;
        }

        public IEnumerable<SquareItem>? MapSquareItems(SearchCatalogObjectsResponse response, SquareClient client, string type)
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
                        string imageURL = "";

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
}
