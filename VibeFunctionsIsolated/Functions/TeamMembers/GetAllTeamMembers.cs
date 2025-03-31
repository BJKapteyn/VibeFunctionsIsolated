using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using VibeFunctionsIsolated.Models.Square;
using VibeFunctionsIsolated.Utility.UtilityInterfaces;

namespace VibeFunctionsIsolated.Functions.TeamMembers
{
    public class GetAllTeamMembers
    {
        private readonly ILogger<GetAllTeamMembers> logger;
        private readonly ISquareUtility squareUtility;
        private readonly IApplicationUtility applicationUtility;

        public GetAllTeamMembers(ILogger<GetAllTeamMembers> logger, ISquareUtility squareUtility, IApplicationUtility applicationUtility)
        {
            this.logger = logger;
            this.squareUtility = squareUtility;
            this.applicationUtility = applicationUtility;
        }

        [Function("GetAllTeamMembers")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {

            IEnumerable<SquareTeamMember> teamMembers = await squareUtility.MapAllBookableTeamMembers();

            if(teamMembers.Any() == false)
            {;
                logger.LogError("No team members were found calling {0}", nameof(GetAllTeamMembers));

                return new NotFoundResult();
            }

            return new OkObjectResult(teamMembers);
        }
    }
}
