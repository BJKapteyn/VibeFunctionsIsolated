using Microsoft.Azure.Cosmos;

namespace VibeFunctionsIsolated.Models.Interfaces;

/// <summary>
/// Represents a Cosmos DB item with a unique identifier.
/// </summary>
public interface ICosmosItem
{
#pragma warning disable IDE1006 // Naming Styles
    public string id { get; set; }
#pragma warning restore IDE1006 // Naming Styles    
}
