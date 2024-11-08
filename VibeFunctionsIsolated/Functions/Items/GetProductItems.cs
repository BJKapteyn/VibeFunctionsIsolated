using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Square;
using Square.Exceptions;
using Square.Models;
using VibeCollectiveFunctions.Models;
using VibeCollectiveFunctions.Utility;
using VibeFunctionsIsolated.DAL;
using VibeFunctionsIsolated.Enums;

namespace VibeFunctionsIsolated.Functions.Items
{
    // Gets all shop items
    internal class GetProductItems
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

            SearchCatalogItemsResponse? response = await squareDAL.SearchCatalogItem(body);

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
