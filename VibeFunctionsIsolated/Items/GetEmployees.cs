using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Square;
using Square.Exceptions;
using Square.Models;
using System.Text.Json;
using VibeCollectiveFunctions.Models;
using VibeCollectiveFunctions.Utility;
using VibeFunctionsIsolated.Enums;


namespace VibeCollectiveFunctions.Items
{
    internal class GetEmployees
    {
        private readonly ILogger<GetEmployees> logger;
        private ISquareUtility squareUtility;
        private SquareClient client;

        public GetEmployees(ILogger<GetEmployees> l, ISquareUtility squareUtility)
        {
            logger = l;
            this.squareUtility = squareUtility;
            this.client = squareUtility.InitializeClient();
        }

        [Function("GetEmployees")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            SearchCatalogItemsResponse? response;
            SearchCatalogItemsRequest requestBody = buildRequestBody();

            try
            {
                response = await client.CatalogApi.SearchCatalogItemsAsync(body: requestBody);
            }
            catch (ApiException e)
            {
                Console.WriteLine($"Response Code: {e.ResponseCode}");
                Console.WriteLine($"Exception: {e.Message}");

                return new BadRequestResult();
            }

            List<SquareEmployee>? employees = modelEmployees(response);

            if (employees == null) 
            {
                return new NotFoundResult();
            }

            string json = JsonSerializer.Serialize(employees);

            return new OkObjectResult(json);
        }

        // Pair down response data to limit data exposure
        // response - response from the square API
        private List<SquareEmployee>? modelEmployees(SearchCatalogItemsResponse response)
        {
            if(response.Items == null || response.Items.Count <= 0)
            {
                return null;
            }

            List<SquareEmployee> squareEmployees = response.Items
                .Select(responseItem =>
                {
                    // The custom attribute dictionary uses unpredictable keys so I'm using linq on the list instead to get values during construction
                    List<CatalogCustomAttributeValue> customAttributeValues = responseItem.ItemData.Variations[0].CustomAttributeValues.Values.ToList();
                    string? imageId;
                    if(responseItem.ItemData?.ImageIds != null)
                    {
                        imageId = responseItem.ItemData?.ImageIds[0];
                    }
                    else
                    {
                        imageId = "";
                    }
                    string imageURL = squareUtility.GetImageURL(imageId, client, logger);
                    
                    return new SquareEmployee(responseItem, customAttributeValues, imageURL);
                })
                .ToList();

            return squareEmployees;
        }

        // Add ids and product type to narrow down results
        private SearchCatalogItemsRequest buildRequestBody()
        {
            var categoryIds = new List<string>()
            {
                // Id for Employee category
                "BJMQNUV2IRXQ4LQLY3BD72ED"
            };

            var productTypes = new List<string>()
            {
                SquareProductType.AppointmentsService
            };

            SearchCatalogItemsRequest body = new SearchCatalogItemsRequest.Builder()
              .CategoryIds(categoryIds)
              .ProductTypes(productTypes)
              .Build();

            return body;
        }
    }
}
