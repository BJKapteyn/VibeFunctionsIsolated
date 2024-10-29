using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Square.Models;
using Square;
using VibeCollectiveFunctions.Utility;
using static VibeCollectiveFunctions.Enums.SquareEnums;

namespace VibeCollectiveFunctions.Functions.Items
{
    internal class GetItemCategories
    {
        private readonly ILogger<GetItemCategories> _logger;
        private readonly ISquareUtility SquareUtility;

        public GetItemCategories(ILogger<GetItemCategories> logger, ISquareUtility squareUtility)
        {
            SquareUtility = squareUtility;
            _logger = logger;
        }

        [Function("GetItemCategories")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            SquareClient client = SquareUtility.InitializeClient();
            SearchCatalogObjectsResponse response;

            List<string> objectTypes = new()
            {
                CatalogObjectType.CATEGORY.ToString(),
            };

            SearchCatalogObjectsRequest? requestBody = new SearchCatalogObjectsRequest.Builder()
                .ObjectTypes(objectTypes)
                .Build();

            try
            {
                response = await client.CatalogApi.SearchCatalogObjectsAsync(requestBody);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }



            return new OkObjectResult("yay");
        }
    }
}
