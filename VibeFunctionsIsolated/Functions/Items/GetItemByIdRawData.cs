using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using VibeFunctionsIsolated.Functions.Items;
using VibeFunctionsIsolated.Utility;
using VibeFunctionsIsolated.DAL;
using VibeFunctionsIsolated.Models;

namespace VibeCollectiveIsolated.Functions.Items;

public class GetItemByIdRawData
{
    private readonly ILogger<GetItems> logger;
    private readonly ISquareUtility squareUtility;
    private readonly ISquareDAL squareDAL;

    public GetItemByIdRawData(ILogger<GetItems> logger, ISquareUtility squareUtility, ISquareDAL squareDAL)
    {
        this.logger = logger;
        this.squareUtility = squareUtility;
        this.squareDAL = squareDAL;
    }

    [Function("GetItemByIdRawData")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        CatalogInformation? categoryInfo = await squareUtility.DeserializeStream<CatalogInformation>(req.Body);

        if (categoryInfo == null)
        {
            logger.LogError($"{nameof(GetItemByIdRawData)} request wasn't formatted correctly");
            return new BadRequestResult();
        }

        IEnumerable<SquareItemRawData> stuff = await squareDAL.GetItemsByIdRawData(categoryInfo);

        return new OkObjectResult("Welcome to Azure Functions!");
    }
}
