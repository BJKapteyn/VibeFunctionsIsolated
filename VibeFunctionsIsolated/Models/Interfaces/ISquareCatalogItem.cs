namespace VibeFunctionsIsolated.Models.Interfaces;

/// <summary>
/// Represents a catalog item in the Square catalog.
/// </summary>
public interface ISquareCatalogItem
{
    public string Id { get; set; }
    public string Name { get; set; } 
    public string? Description { get; set; }
    string? ImageURL { get; set; }
}

