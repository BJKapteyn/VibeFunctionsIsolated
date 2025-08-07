namespace VibeFunctionsIsolated.Models.Cosmos;

public class CosmosItemId
{
    public CosmosItemId(string id, string partitionKey)
    {
        this.id = id;
        this.partitionKey = partitionKey;
    }

    public string id { get; set; }
    public string partitionKey { get; set; }
}
