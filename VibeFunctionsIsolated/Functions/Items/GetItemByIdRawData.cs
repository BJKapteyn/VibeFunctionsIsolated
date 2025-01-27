using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using VibeFunctionsIsolated.Utility;
using VibeFunctionsIsolated.Models;
using VibeFunctionsIsolated.DAL.Interfaces;

namespace VibeFunctionsIsolated.Functions.Items;

public class GetItemByIdRawData
{
    private readonly ILogger<GetItems> logger;
    private readonly ISquareUtility squareUtility;
    private readonly ISquareApiDataAccess squareApiDal;

    public GetItemByIdRawData(ILogger<GetItems> logger, ISquareUtility squareUtility, ISquareApiDataAccess squareApiDal)
    {
        this.logger = logger;
        this.squareUtility = squareUtility;
        this.squareApiDal = squareApiDal;
    }

    [Function("GetItemByIdRawData")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        CatalogInformation? categoryInfo = await squareUtility.DeserializeStream<CatalogInformation>(req.Body);

        if (categoryInfo is null)
        {
            logger.LogError($"{nameof(GetItemByIdRawData)} request wasn't formatted correctly");
            return new BadRequestResult();
        }

        IEnumerable<SquareItemRawData> response = await squareApiDal.GetSquareAPIRawData(categoryInfo);

        if(response.Any() is false)
        {
            return new NotFoundResult();
        }

        return new OkObjectResult(response);
    }
}
