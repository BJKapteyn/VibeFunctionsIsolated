using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Square.Models;
using VibeFunctionsIsolated.Utility;
using VibeFunctionsIsolated.DAL;
using VibeFunctionsIsolated.Models;
using static VibeFunctionsIsolated.Enums.SquareEnums;

namespace VibeFunctionsIsolated.Functions.Items
{
    public class GetItemCategories
    {
        private readonly ILogger<GetItemCategories> _logger;
        private readonly ISquareUtility squareUtility;
        private readonly ISquareDAL squareDAL;

        public GetItemCategories(ILogger<GetItemCategories> logger, ISquareUtility squareUtility, ISquareDAL squareDAL)
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

            IEnumerable<SquareCategory> catalogItems = response.Objects.Select(catalogItem =>
            {
                string? imageId = catalogItem.CategoryData.ImageIds == null ? null : catalogItem.CategoryData.ImageIds[0];
                string? imageURL = squareDAL.GetImageURL(imageId).Result;

                return new SquareCategory(catalogItem, imageURL);
            });

            return new OkObjectResult(catalogItems);
        }
    }
}
