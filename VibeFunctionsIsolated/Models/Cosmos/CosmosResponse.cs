using System.Net;
using VibeFunctionsIsolated.Models.Interfaces;

namespace VibeFunctionsIsolated.Models.Cosmos;

public class CosmosResponse
{
    public CosmosResponse(HttpStatusCode statusCode, ICosmosItem? cosmosItem)
    {
        StatusCode = statusCode;
        CosmosItem = cosmosItem;
    }
    public HttpStatusCode StatusCode { get; set; }
    public ICosmosItem? CosmosItem { get; set; }
}
