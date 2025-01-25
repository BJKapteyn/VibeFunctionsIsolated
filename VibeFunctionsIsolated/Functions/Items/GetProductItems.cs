using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Square.Models;
using VibeFunctionsIsolated.Models;
using VibeFunctionsIsolated.Utility;
using VibeFunctionsIsolated.DAL;
using VibeFunctionsIsolated.Enums;

namespace VibeFunctionsIsolated.Functions.Items
{
    // Gets all shop items
    public class GetProductItems
    {
        private readonly ILogger<GetProductItems> _logger;
        private readonly ISquareUtility squareUtility;
        private readonly ISquareDAL squareDAL;

        public GetProductItems(ILogger<GetProductItems> logger, ISquareUtility squareUtility, ISquareDAL squareDAL)
        {
            _logger = logger;
            this.squareUtility = squareUtility;
            this.squareDAL = squareDAL;
        }

        [Function("GetProductItems")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            var productTypes = new List<string>()
            {
                SquareProductType.Regular
            };

            SearchCatalogItemsRequest body = new SearchCatalogItemsRequest.Builder()
              .ProductTypes(productTypes)
              .Build();

            SearchCatalogItemsResponse? response = await squareDAL.SearchCatalogItems(body);

            if (response == null)
            {
                return new BadRequestResult();
            }

            IEnumerable<SquareItem> productItems = response.Items.Select(productItem =>
            {
                string? imageId = null;
                if (productItem.ItemData.ImageIds != null)
                    imageId = squareDAL.GetImageURL(productItem.ItemData.ImageIds[0]).Result;

                return new SquareItem(productItem, imageId);
            });

            return new OkObjectResult(productItems);
        }
    }
}
