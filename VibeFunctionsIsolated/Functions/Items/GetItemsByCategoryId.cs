using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Square.Models;
using VibeFunctionsIsolated.DAL.Interfaces;
using VibeFunctionsIsolated.Models.Square;
using VibeFunctionsIsolated.Utility.UtilityInterfaces;

namespace VibeFunctionsIsolated.Functions.Items;

public class GetItemsByCategoryId
{
    private readonly ILogger<GetItemsByCategoryId> _logger;
    private readonly ISquareUtility squareUtility;
    private readonly ISquareSdkDataAccess squareSdkDal;
    private readonly ISquareApiDataAccess squareApiDal;
    private readonly IApplicationUtility applicationUtility;

    public GetItemsByCategoryId(ILogger<GetItemsByCategoryId> logger,
        ISquareUtility squareUtility,
        ISquareSdkDataAccess squareSdkDal,
        ISquareApiDataAccess squareApiDal,
        IApplicationUtility applicationUtility)
    {
        _logger = logger;
        this.squareUtility = squareUtility;
        this.squareApiDal = squareApiDal;
        this.squareSdkDal = squareSdkDal;
        this.applicationUtility = applicationUtility;
    }

    [Function("GetItemsByCategoryId")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {        
        CatalogInformation? categoryId = null;

        categoryId = await applicationUtility.DeserializeStream<CatalogInformation?>(req.Body);

        if (categoryId == null)
        {
            _logger.LogError($"{nameof(GetItemsByCategoryId)} request wasn't formatted correctly");
            return new BadRequestResult();
        }

        SearchCatalogItemsResponse? response = await squareSdkDal.SearchCatalogItemsByCategoryId(categoryId);

        if (response?.Errors != null || response == null)
        {
            _logger.LogError($"{nameof(GetItemsByCategoryId)} errors: /n{response?.Errors.ToString()}");
            return new BadRequestResult();
        }

        IEnumerable<SquareItem> squareItems = await squareUtility.MapCatalogObjectsToLocalModel(response.Items, true);

        if (categoryId.ReportingCategoryId != null)
        {
            squareItems = squareUtility.GetItemsByReportingCategoryId(squareItems, categoryId.ReportingCategoryId);
        }

        return new OkObjectResult(squareItems);
    }
}

