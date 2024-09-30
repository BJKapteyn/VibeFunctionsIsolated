using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Square;
using Square.Authentication;
using Square.Exceptions;
using Square.Models;
using VibeCollectiveFunctions.Enums;
using VibeCollectiveFunctions.Models;
using VibeCollectiveFunctions.Utility;

namespace VibeCollectiveFunctions.Items
{
    internal class GetItems
    {
        private readonly ILogger<GetItems> logger;
        private readonly ISquareUtility SquareUtility;

        public GetItems(ILogger<GetItems> _logger, ISquareUtility squareUtility)
        {
            SquareUtility = squareUtility;
            logger = _logger;
        }

        [Function("GetItems")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            BearerAuthModel bearerAuth = new BearerAuthModel.Builder(System.Environment.GetEnvironmentVariable("SquareProduction")).Build();
            ListCatalogResponse response;
            List<SquareItem> squareItems = new List<SquareItem>();

            SquareClient client = SquareUtility.InitializeClient();
            
            try
            {
                response = await client.CatalogApi.ListCatalogAsync();
            }
            catch (ApiException e)
            {
                logger.LogError(e.Message);
                Console.WriteLine($"Response Code: {e.ResponseCode}");
                Console.WriteLine($"Exception: {e.Message}");

                return new NotFoundResult();
            }

            if (response.Objects.Count > 0)
            {
                squareItems = response.Objects.Where(responseItem => responseItem.Type == SquareEnums.CatalogObjectType.ITEM.ToString())
                    .Select(responseItem =>
                    {
                        return new SquareItem(responseItem);

                    })
                    .ToList();
            }

            return new OkObjectResult(squareItems);
        }
    }
}
