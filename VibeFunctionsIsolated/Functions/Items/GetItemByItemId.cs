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

public class GetItemByItemId(ILogger<GetItemByItemId> logger, ISquareUtility squareUtility, ISquareSdkDataAccess squareDal)
{
    private readonly ILogger<GetItemByItemId> _logger = logger; 
    private readonly ISquareUtility squareUtility = squareUtility;
    private readonly ISquareSdkDataAccess squareDal = squareDal;

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

        RetrieveCatalogObjectResponse? response = await squareDal.GetCatalogObjectById(itemId);

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
