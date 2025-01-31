using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Square.Models;
using VibeFunctionsIsolated.Utility;
using VibeFunctionsIsolated.Models;
using static VibeFunctionsIsolated.Enums.SquareEnums;
using VibeFunctionsIsolated.DAL.Interfaces;

namespace VibeFunctionsIsolated.Functions.Items
{
    public class GetItemCategories
    {
        private readonly ILogger<GetItemCategories> _logger;
        private readonly ISquareUtility squareUtility;
        private readonly ISquareSdkDataAccess squareDAL;

        public GetItemCategories(ILogger<GetItemCategories> logger, ISquareUtility squareUtility, ISquareSdkDataAccess squareDAL)
        {
            _logger = logger;
            this.squareUtility = squareUtility;
            this.squareDAL = squareDAL;
        }

        [Function("GetItemCategories")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            List<string> objectTypes = new()
            {
                CatalogObjectType.CATEGORY.ToString(),
            };

            SearchCatalogObjectsRequest? requestBody = new SearchCatalogObjectsRequest.Builder()
                .ObjectTypes(objectTypes)
                .Build();

            SearchCatalogObjectsResponse? response = await squareDAL.SearchCatalogObjects(requestBody);
            
            if (response == null)
            {
                return new NotFoundResult();
            }

            Dictionary<string, Task<string>> itemIdToExtraData = [];

            IEnumerable<SquareCategory> catalogItems = response.Objects.Select(catalogItem =>
            {
                string? imageId = catalogItem.CategoryData.ImageIds == null ? null : catalogItem.CategoryData.ImageIds[0];

                Task<string> imageUrlTask = squareDAL.GetImageURL(imageId);

                itemIdToExtraData.TryAdd(catalogItem.Id, imageUrlTask);

                return new SquareCategory(catalogItem, "");
            }).ToList();

            Task.WaitAll(itemIdToExtraData.Values.ToArray());

            foreach (SquareCategory category in catalogItems)
            {
                string imageUrl = itemIdToExtraData[category.Id].Result;

                category.ImageURL = imageUrl;
            }

            return new OkObjectResult(catalogItems);
        }
    }
}
