using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Square;
using Square.Authentication;
using Square.Exceptions;
using Square.Models;
using VibeFunctionsIsolated.Models;
using VibeFunctionsIsolated.Utility.UtilityInterfaces;

namespace VibeFunctionsIsolated.Functions.Email;

public class InsertCustomerEmail
{
    private readonly ILogger<InsertCustomerEmail> _logger;
    private readonly ISquareUtility squareUtility;
    private readonly IApplicationUtility applicationUtility;

    public InsertCustomerEmail(ILogger<InsertCustomerEmail> logger, ISquareUtility squareUtility, IApplicationUtility applicationUtility)
    {
        _logger = logger;
        this.squareUtility = squareUtility;
        this.applicationUtility = applicationUtility;
    }

    [Function("InsertCustomerEmail")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        BearerAuthModel bearerAuth = new BearerAuthModel.Builder(System.Environment.GetEnvironmentVariable("SquareProduction")).Build();

        if (req?.Body == null) 
        {
            return new BadRequestResult();
        }

        CosmosEmail? cosmosEmail = await applicationUtility.DeserializeStream<CosmosEmail?>(req.Body);

        bool isEmail = VerifyEmail(cosmosEmail?.email);

        if (cosmosEmail == null || cosmosEmail.email == null || !isEmail)
        {
            return new BadRequestObjectResult("Missing Data");
        }

        SquareClient client = new SquareClient.Builder()
            .Environment(Square.Environment.Production)
            .BearerAuthCredentials(bearerAuth)
            .Build();

        CreateCustomerRequest request = new(emailAddress: cosmosEmail.email);
        CreateCustomerResponse response;

        try
        {
            response = client.CustomersApi.CreateCustomer(request);
        }
        catch (ApiException e)
        {
            _logger.LogError(e.Message);
            Console.WriteLine($"Response Code: {e.ResponseCode}");
            Console.WriteLine($"Exception: {e.Message}");
            return new NotFoundResult();
        }

        return new OkObjectResult("function ran to the end");
    }

    private static bool VerifyEmail(string? email)
    {
        if (email == null)
            return false;

        string trimmedEmail = email.Trim();

        if (trimmedEmail.EndsWith('.'))
        {
            return false;
        }
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == trimmedEmail;
        }
        catch
        {
            return false;
        }
    }
}
