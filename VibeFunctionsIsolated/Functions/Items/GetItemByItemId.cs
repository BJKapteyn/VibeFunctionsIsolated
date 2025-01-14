using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using VibeFunctionsIsolated.Utility;
using VibeFunctionsIsolated.DAL;
using VibeFunctionsIsolated.Models;
using Square.Models;

namespace VibeFunctionsIsolated.Functions.Items;

public class GetItemByItemId(ILogger<GetItemByItemId> logger, ISquareUtility squareUtility, ISquareDAL squareDal)
{
    private readonly ILogger<GetItemByItemId> _logger = logger; 
    private readonly ISquareUtility SquareUtility = squareUtility;
    private readonly ISquareDAL SquareDal = squareDal;

    //public GetItemByItemId(ILogger<GetItemByItemId> logger, ISquareUtility squareUtility, ISquareDAL squareDal)
    //{
    //    SquareUtility = squareUtility;
    //    SquareDal = squareDal;
    //    _logger = logger;
    //}

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

        if(response == null || response.RelatedObjects.Any() == false)
        {
            string className = nameof(GetItemByItemId);
            string itemIdString = itemId.Id.ToString();
            _logger.LogError("{className} could not find the item with id {itemIdString}", className, itemIdString);
            return new NotFoundResult();
        }

        List<SquareItem> items = response.RelatedObjects.Select(responseItem =>
        {
            string? imageId = responseItem.ItemData.ImageIds == null ? null : responseItem.ItemData.ImageIds[0];
            string? imageURL = SquareDal.GetImageURL(imageId).Result;

            return new SquareItem(responseItem, imageURL);
        }).ToList();


        return new OkObjectResult("Ok");
    }
}
