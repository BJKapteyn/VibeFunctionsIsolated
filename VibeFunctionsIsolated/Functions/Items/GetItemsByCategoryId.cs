using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using VibeCollectiveFunctions.Utility;

namespace VibeFunctionsIsolated.Functions.Items;

internal class GetItemsByCategoryId
{
    private readonly ILogger<GetItemsByCategoryId> _logger;
    private readonly ISquareUtility SquareUtility;

    public GetItemsByCategoryId(ILogger<GetItemsByCategoryId> logger, ISquareUtility squareUtility)
    {
        _logger = logger;
        SquareUtility = squareUtility;
    }

    [Function("GetItemsByCategory")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("Welcome to Azure Functions!");
    }
}

