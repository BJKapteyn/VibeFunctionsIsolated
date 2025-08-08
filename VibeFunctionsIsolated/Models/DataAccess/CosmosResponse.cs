using System.Net;
using VibeFunctionsIsolated.Models.Interfaces;

namespace VibeFunctionsIsolated.Models.DataAccess;

public class CosmosResponse
{
    public CosmosResponse(HttpStatusCode statusCode, ICosmosItem cosmosItem)
    {
        this.StatusCode = statusCode;
        this.CosmosItem = cosmosItem;
    }
    public HttpStatusCode StatusCode { get; set; }
    public ICosmosItem CosmosItem { get; set; }
}
