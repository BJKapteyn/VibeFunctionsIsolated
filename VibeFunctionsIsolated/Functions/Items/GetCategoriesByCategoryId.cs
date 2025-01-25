using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Square.Models;
using VibeFunctionsIsolated.Utility;
using VibeFunctionsIsolated.DAL;
using VibeFunctionsIsolated.Models;

namespace VibeFunctionsIsolated.Functions.Items
{
    public class GetCategoriesByCategoryId
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
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            CatalogInformation? categoryId = squareUtility.DeserializeStream<CatalogInformation>(req.Body).Result;

            if (categoryId == null)
            {
                _logger.LogError($"{nameof(GetCategoriesByCategoryId)} could not map the category id");
                return new BadRequestResult();
            }

            SearchCatalogObjectsResponse? response = await squareDAL.SearchCategoryObjectsByParentId(categoryId);

            if (response?.Objects == null || response.Objects.Count == 0) 
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
