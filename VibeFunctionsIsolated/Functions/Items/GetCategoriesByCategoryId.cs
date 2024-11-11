using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Square.Models;
using VibeCollectiveFunctions.Utility;
using VibeFunctionsIsolated.DAL;
using VibeFunctionsIsolated.Models;

namespace VibeFunctionsIsolated.Functions.Items
{
    internal class GetCategoriesByCategoryId
    {
        private readonly ILogger<GetCategoriesByCategoryId> _logger;
        private readonly ISquareDAL squareDAL;
        private readonly ISquareUtility squareUtility;

        public GetCategoriesByCategoryId(ILogger<GetCategoriesByCategoryId> logger, ISquareDAL squareDAL, ISquareUtility squareUtility)
        {
            _logger = logger;
            this.squareDAL = squareDAL;
            this.squareUtility = squareUtility;
        }

        [Function("GetCategoriesByCategoryId")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            CategoryId? categoryName = await squareUtility.DeserializeStream<CategoryId>(req.Body);
            if (categoryName == null)
            {
                _logger.LogError($"{nameof(GetCategoriesByCategoryId)} could not map the category id");
                return new BadRequestResult();
            }

            SearchCatalogObjectsResponse? response = await squareDAL.SearchCategoryObjectsByParentId(categoryName);
            if (response == null) 
            {
                _logger.LogError($"{nameof(GetCategoriesByCategoryId)}: request failed");
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
