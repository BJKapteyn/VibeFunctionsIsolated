using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using VibeFunctionsIsolated.DAL.Interfaces;
using VibeFunctionsIsolated.Models.Square;
using VibeFunctionsIsolated.Utility.UtilityInterfaces;

namespace VibeFunctionsIsolated.Functions.TeamMembers
{
    public class GetAllTeamMembers
    {
        private readonly ILogger<GetAllTeamMembers> _logger;
        private readonly ISquareUtility squareUtility;
        private readonly IApplicationUtility applicationUtility;

        public GetAllTeamMembers(ILogger<GetAllTeamMembers> logger, ISquareUtility squareUtility, IApplicationUtility applicationUtility)
        {
            _logger = logger;
            this.squareUtility = squareUtility;
            this.applicationUtility = applicationUtility;
        }

        [Function("GetAllTeamMembers")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {


            IEnumerable<SquareTeamMember> teamMembers = await squareUtility.GetAllTeamMembersWithDetails();

            return new OkObjectResult(teamMembers);
        }
    }
}
