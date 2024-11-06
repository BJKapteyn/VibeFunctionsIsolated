using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Square;
using Square.Exceptions;
using Square.Models;
using VibeCollectiveFunctions.Utility;
using VibeFunctionsIsolated.Enums;

namespace VibeFunctionsIsolated.Functions.Items
{
    // Gets all shop items
    internal class GetProductItems
    {
        private readonly ILogger<GetProductItems> _logger;
        private readonly ISquareUtility SquareUtility;

        public GetProductItems(ILogger<GetProductItems> logger, ISquareUtility squareUtility)
        {
            SquareUtility = squareUtility;
            _logger = logger;
        }

        [Function("GetProductItems")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            SquareClient client = SquareUtility.InitializeClient();
            SearchCatalogItemsResponse response;
            var productTypes = new List<string>()
            {
                SquareProductType.Regular
            };

            var body = new SearchCatalogItemsRequest.Builder()
              .ProductTypes(productTypes)
              .Build();

            try
            {
                response = await client.CatalogApi.SearchCatalogItemsAsync(body: body);
            }
            catch (ApiException e)
            {
                Console.WriteLine($"{nameof(GetProductItems)} Response Code: {e.ResponseCode}");
                Console.WriteLine($"Exception: {e.Message}");
            }

            return new OkObjectResult(response);
        }
    }
}
