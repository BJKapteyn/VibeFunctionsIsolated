using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using VibeCollectiveFunctions.Functions.Items;
using VibeCollectiveFunctions.Utility;
using VibeFunctionsIsolated.DAL;

namespace VibeFunctionsIsolated.Functions.Items
{
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
            string stuff = await squareDAL.GetItemsByIdRawData();

            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
