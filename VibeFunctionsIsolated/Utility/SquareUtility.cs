using Square.Models;
using System.Text.Json;
using VibeCollectiveFunctions.Models;
using VibeFunctionsIsolated.DAL;
using VibeFunctionsIsolated.Enums;
using static VibeCollectiveFunctions.Enums.SquareEnums;

namespace VibeCollectiveFunctions.Utility
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
    }
}
