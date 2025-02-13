namespace VibeFunctionsIsolated.Models.Interfaces;

public interface ISquareCatalogItem
{
    public string Id { get; set; }
    public string Name { get; set; } 
    public string? Description { get; set; }
    string? ImageURL { get; set; }
}

