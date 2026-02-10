namespace VibeFunctionsIsolated.Models.Interfaces;

/// <summary>
/// Represents a Cosmos DB item with a unique identifier.
/// </summary>
public interface ICosmosItem
{
    public string id { get; set; }
}
