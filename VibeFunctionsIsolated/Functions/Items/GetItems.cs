using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Square;
using Square.Exceptions;
using Square.Models;
using VibeCollectiveFunctions.Models;
using VibeCollectiveFunctions.Utility;
using VibeFunctionsIsolated.Enums;
using static VibeCollectiveFunctions.Enums.SquareEnums;

namespace VibeCollectiveFunctions.Functions.Items;

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
        SquareClient client = SquareUtility.InitializeClient();
        SearchCatalogObjectsResponse response;

        List<string> objectTypes = new()
        {
            CatalogObjectType.ITEM.ToString(),
            CatalogObjectType.CATEGORY.ToString(),
        };

        SearchCatalogObjectsRequest? requestBody = new SearchCatalogObjectsRequest.Builder()
            .ObjectTypes(objectTypes)
            .Build();

        try
        {
            response = await client.CatalogApi.SearchCatalogObjectsAsync(requestBody);
        }
        catch (ApiException e)
        {
            logger.LogError(e.Message);
            Console.WriteLine($"Response Code: {e.ResponseCode}");
            Console.WriteLine($"Exception: {e.Message}");

            return new NotFoundResult();
        }

        IEnumerable<SquareItem>? squareItems = SquareUtility.MapSquareItems(response, client, CatalogObjectType.ITEM.ToString()); 
        if(squareItems == null || squareItems.Count() == 0)
        {
            return new BadRequestObjectResult(squareItems);
        }

        return new OkObjectResult(squareItems); 
    }

} 
