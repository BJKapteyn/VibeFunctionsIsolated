using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Square.Models;
using VibeCollectiveFunctions.Models;
using VibeCollectiveFunctions.Utility;
using VibeFunctionsIsolated.DAL;
using VibeFunctionsIsolated.Models;

namespace VibeFunctionsIsolated.Functions.Items;

internal class GetItemsByCategoryId
{
    private readonly ILogger<GetItemsByCategoryId> _logger;
    private readonly ISquareUtility squareUtility;
    private readonly ISquareDAL squareDAL;

    public GetItemsByCategoryId(ILogger<GetItemsByCategoryId> logger, ISquareUtility squareUtility, ISquareDAL squareDAL)
    {
        _logger = logger;
        this.squareUtility = squareUtility;
        this.squareDAL = squareDAL;
    }

    [Function("GetItemsByCategoryId")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {        
        CategoryId? categoryId = null;
        try
        {
            categoryId = await squareUtility.DeserializeStream<CategoryId?>(req.Body);
        }
        catch(Exception e)
        {
            _logger.LogError(e.Message);
        }

        if (categoryId == null)
        {
            _logger.LogError($"{nameof(GetItemsByCategoryId)} request wasn't formatted correctly");
            return new BadRequestResult();
        }

        SearchCatalogItemsResponse? response = await squareDAL.SearchCatalogItemsByCategoryId(categoryId);

        if (response == null || response.Items == null)
        {
            _logger.LogError($"{nameof(GetItemsByCategoryId)}: request returned no items");
            return new NotFoundResult();
        }

        if(response.Errors != null)
        {
            _logger.LogError($"{nameof(GetItemsByCategoryId)} errors: /n{response?.Errors.ToString()}");
            return new BadRequestResult();
        }

        IEnumerable<SquareItem> items = response.Items.Select(responseItem =>
        {
            string? imageId = responseItem.ItemData.ImageIds == null ? null : responseItem.ItemData.ImageIds[0];
            string? imageURL = squareDAL.GetImageURL(imageId).Result;

            return new SquareItem(responseItem, imageURL);
        });

        return new OkObjectResult(items);
    }
}

