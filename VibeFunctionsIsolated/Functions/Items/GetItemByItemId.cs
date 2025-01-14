using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using VibeFunctionsIsolated.Utility;
using VibeFunctionsIsolated.DAL;
using VibeFunctionsIsolated.Models;
using Square.Models;

namespace VibeFunctionsIsolated.Functions.Items;

public class GetItemByItemId
{
    private readonly ILogger<GetItemByItemId> _logger; 
    private readonly ISquareUtility SquareUtility;
    private readonly ISquareDAL SquareDal;

    public GetItemByItemId(ILogger<GetItemByItemId> logger, ISquareUtility squareUtility, ISquareDAL squareDal)
    {
        SquareUtility = squareUtility;
        SquareDal = squareDal;
        _logger = logger;
    }

    [Function("GetItemByItemId")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        ItemId? itemId = await SquareUtility.DeserializeStream<ItemId>(req.Body);

        if (itemId == null)
        {
            string className = nameof(GetItemByItemId);
            _logger.LogError("{className} could not map the category id", className);
            return new BadRequestResult();
        }

        RetrieveCatalogObjectResponse? response = await SquareDal.GetCatalogObjectById(itemId);

        if(response == null)
        {
            string className = nameof(GetItemByItemId);
            string itemIdString = itemId.Id.ToString();
            _logger.LogError("{className} could not find the item with id {itemIdString}", className, itemIdString);
            return new NotFoundResult();
        }





        return new OkObjectResult("Ok");
    }
}
