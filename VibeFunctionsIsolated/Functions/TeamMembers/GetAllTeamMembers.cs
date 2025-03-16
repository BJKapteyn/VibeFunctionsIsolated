using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using VibeFunctionsIsolated.DAL.Interfaces;

namespace VibeFunctionsIsolated.Functions.TeamMembers
{
    public class GetAllTeamMembers
    {
        private readonly ILogger<GetAllTeamMembers> _logger;
        private readonly ISquareSdkDataAccess squareSdkDataAccess;

        public GetAllTeamMembers(ILogger<GetAllTeamMembers> logger, ISquareSdkDataAccess squareSdkDataAccess)
        {
            _logger = logger;
            this.squareSdkDataAccess = squareSdkDataAccess;
        }

        [Function("GetAllTeamMembers")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
