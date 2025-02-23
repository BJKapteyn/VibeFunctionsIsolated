using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Square.Models;
using VibeFunctionsIsolated.DAL.Interfaces;
using VibeFunctionsIsolated.Models.Square;
using VibeFunctionsIsolated.Utility;
using static VibeFunctionsIsolated.Enums.SquareEnums;

namespace VibeFunctionsIsolated.Functions.Items;

public class GetItems
{
    private readonly ISquareUtility squareUtility;
    private readonly ISquareSdkDataAccess squareDAL;
    public GetItems(ISquareUtility squareUtility, ISquareSdkDataAccess squareDAL)
    {
        this.squareUtility = squareUtility;
        this.squareDAL = squareDAL;
    }

    [Function("GetItems")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        List<string> objectTypes = new()
        {
            CatalogObjectType.ITEM.ToString(),
            CatalogObjectType.CATEGORY.ToString(),
        };

        SearchCatalogObjectsRequest? requestBody = new SearchCatalogObjectsRequest.Builder()
            .ObjectTypes(objectTypes)
            .Build();

        SearchCatalogObjectsResponse? response = await squareDAL.SearchCatalogObjects(requestBody);
        if (response == null)
        {
            return new NotFoundResult();
        }

        IEnumerable<SquareItem>? squareItems = squareUtility.MapSquareProductItems(response, CatalogObjectType.ITEM.ToString());
        if (squareItems == null || squareItems.Any())
        {
            return new BadRequestResult();
        }

        return new OkObjectResult(squareItems);
    }

}
