using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Square.Models;
using VibeFunctionsIsolated.DAL.Interfaces;
using VibeFunctionsIsolated.Models;
using VibeFunctionsIsolated.Models.Interfaces;
using VibeFunctionsIsolated.Utility;

namespace VibeFunctionsIsolated.Functions.Items;

/// <summary>
/// Gets single item by item id
/// </summary>
/// <param name="logger">Logger for logging errors</param>
/// <param name="squareUtility">Injected utility class for square related work</param>
/// <param name="squareDal">Injected class for data retrieval from the Square API</param>
public class GetItemByItemId(ILogger<GetItemByItemId> logger, ISquareUtility squareUtility, ISquareSdkDataAccess squareDal)
{
    private readonly ILogger<GetItemByItemId> _logger = logger; 
    private readonly ISquareUtility squareUtility = squareUtility;
    private readonly ISquareSdkDataAccess squareSdkDal = squareDal;

    [Function("GetItemByItemId")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        CatalogInformation? itemId = await squareUtility.DeserializeStream<CatalogInformation>(req.Body);

        if (itemId == null)
        {
            string className = nameof(GetItemByItemId);
            _logger.LogError("{className} could not map the category id", className);
            return new BadRequestResult();
        }

        RetrieveCatalogObjectResponse? response = await squareSdkDal.GetCatalogObjectById(itemId);

        if (response?.MObject?.ItemData == null)
        {
            string className = nameof(GetItemByItemId);
            string itemIdString = itemId.Id.ToString();
            _logger.LogError("{className} could not find the item with id {itemIdString}", className, itemIdString);
            return new NotFoundResult();
        }

        ISquareCatalogItem? item = squareUtility.GetItemFromCatalogObjectResponse(response);

        return new OkObjectResult(item);
    }
}
