using System.Text.Json.Serialization;

namespace VibeFunctionsIsolated.Models.Cosmos.UtilityModels;

[JsonSerializable(typeof(CosmosDeleteItemId))]
public class CosmosDeleteItemId : CosmosItemId
{
    [JsonConstructor]
    public CosmosDeleteItemId(string squareEventId, string id, string partitionKey) : base(id, partitionKey)
    {
        this.squareEventId = squareEventId;
    }

    /// <summary>
    /// Id of Square event to be deleted
    /// </summary>
    [JsonPropertyName("squareEventId")]
    public string squareEventId { get; set; }

}
