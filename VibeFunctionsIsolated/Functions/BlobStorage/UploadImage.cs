using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using VibeFunctionsIsolated.Utility.UtilityInterfaces;

namespace VibeFunctionsIsolated.Functions.BlobStorage;

public class UploadImage
{
    private readonly ILogger<UploadImage> _logger;
    private readonly IApplicationUtility applicationUtility;

    public UploadImage(ILogger<UploadImage> logger, IApplicationUtility applicationUtility)
    {
        _logger = logger;
        this.applicationUtility = applicationUtility;
    }

    [Function("UploadImage")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        // Example usage:

        string imageUrl = await applicationUtility.UploadImage(req.Body);
        return new OkObjectResult(imageUrl);
    }
}