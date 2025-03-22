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
            //IEnumerable<string>? teamMemberIds = await applicationUtility.DeserializeStream<IEnumerable<string>>(req.Body);

            //if(teamMemberIds is null || teamMemberIds.Count() == 0)
            //{
            //    _logger.LogError("{0} bad http request", nameof(GetAllTeamMembers));
            //    return new BadRequestResult();
            //}

            IEnumerable<SquareTeamMember> teamMembers = await squareUtility.GetAllTeamMembersWithDetails();

            



            return new OkObjectResult(teamMembers);
        }
    }
}
