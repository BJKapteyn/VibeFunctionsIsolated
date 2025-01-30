using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Square.Models;
using VibeFunctionsIsolated.Models;
using VibeFunctionsIsolated.Utility;
using VibeFunctionsIsolated.DAL.Interfaces;

namespace VibeFunctionsIsolated.Functions.Items;

public class GetItemsByCategoryId
{
    private readonly ILogger<GetItemsByCategoryId> _logger;
    private readonly ISquareUtility squareUtility;
    private readonly ISquareSdkDataAccess squareSdkDal;
    private readonly ISquareApiDataAccess squareApiDal;

    public GetItemsByCategoryId(ILogger<GetItemsByCategoryId> logger,
        ISquareUtility squareUtility,
        ISquareSdkDataAccess squareSdkDal,
        ISquareApiDataAccess squareApiDal)
    {
        _logger = logger;
        this.squareUtility = squareUtility;
        this.squareApiDal = squareApiDal;
        this.squareSdkDal = squareSdkDal;
    }

    [Function("GetItemsByCategoryId")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {        
        CatalogInformation? categoryId = null;

        categoryId = await squareUtility.DeserializeStream<CatalogInformation?>(req.Body);

        if (categoryId == null)
        {
            _logger.LogError($"{nameof(GetItemsByCategoryId)} request wasn't formatted correctly");
            return new BadRequestResult();
        }

        SearchCatalogItemsResponse? response = await squareSdkDal.SearchCatalogItemsByCategoryId(categoryId);

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
            string imageId = responseItem.ItemData.ImageIds == null ? "" : responseItem.ItemData.ImageIds[0];

            Task<string> imageUrlTask = squareSdkDal.GetImageURL(imageId);
            Task<string> buyNowLinkTask = squareApiDal.GetBuyNowLink(responseItem.Id);

            Task.WaitAll(imageUrlTask, buyNowLinkTask);

            string imageURL = imageUrlTask.Result;
            string buyNowLink = buyNowLinkTask.Result;

            return new SquareItem(responseItem, imageURL, buyNowLink);
        }).ToList();

        if(categoryId.ReportingCategoryId != null)
        {
            items = squareUtility.GetItemsWithReportingCategoryId(items, categoryId.ReportingCategoryId);
        }

        return new OkObjectResult(items);
    }
}

