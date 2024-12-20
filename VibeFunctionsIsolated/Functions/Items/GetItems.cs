﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Square.Models;
using VibeCollectiveFunctions.Models;
using VibeCollectiveFunctions.Utility;
using VibeFunctionsIsolated.DAL;
using static VibeCollectiveFunctions.Enums.SquareEnums;

namespace VibeCollectiveFunctions.Functions.Items;

internal class GetItems
{
    private readonly ILogger<GetItems> logger;
    private readonly ISquareUtility squareUtility;
    private readonly ISquareDAL squareDAL;
    public GetItems(ILogger<GetItems> _logger, ISquareUtility squareUtility, ISquareDAL squareDAL)
    {
        logger = _logger;
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
        if(squareItems == null || squareItems.Count() == 0)
        {
            return new BadRequestObjectResult(squareItems);
        }

        return new OkObjectResult(squareItems); 
    }

} 
