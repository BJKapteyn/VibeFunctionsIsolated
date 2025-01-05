using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Square.Models;
using System.Text.Json;
using VibeCollectiveFunctions.Models;
using VibeCollectiveFunctions.Utility;
using VibeFunctionsIsolated.DAL;
using VibeFunctionsIsolated.Enums;


namespace VibeCollectiveFunctions.Functions.Items
{
    public class GetEmployees
    {
        private readonly ISquareUtility squareUtility;
        private readonly ISquareDAL squareDAL;

        public GetEmployees(ISquareUtility squareUtility, ISquareDAL squareDAL)
        {
            this.squareUtility = squareUtility;
            this.squareDAL = squareDAL;
        }

        [Function("GetEmployees")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            SearchCatalogItemsRequest requestBody = buildRequestBody();
            SearchCatalogItemsResponse? response = await squareDAL.SearchCatalogItems(requestBody);

            IEnumerable<SquareEmployee>? employees = modelEmployees(response);

            if (employees == null) 
            {
                return new NotFoundResult();
            }

            string json = JsonSerializer.Serialize(employees);

            return new OkObjectResult(json);
        }

        // Pair down response data to limit data exposure
        // response - response from the square API
        private IEnumerable<SquareEmployee>? modelEmployees(SearchCatalogItemsResponse? response)
        {
            if(response?.Items == null || response.Items.Count <= 0)
            {
                return null;
            }

            IEnumerable<SquareEmployee> squareEmployees = response.Items
                .Select(responseItem =>
                {
                    // The custom attribute dictionary uses unpredictable keys so I'm using linq on the list instead to get values during construction
                    IEnumerable<CatalogCustomAttributeValue> customAttributeValues = responseItem.ItemData.Variations[0].CustomAttributeValues.Values;
                    string? imageId;
                    if(responseItem.ItemData?.ImageIds != null)
                    {
                        imageId = responseItem.ItemData?.ImageIds[0];
                    }
                    else
                    {
                        imageId = "";
                    }
                    string? imageURL = squareDAL.GetImageURL(imageId).Result;
                    
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
