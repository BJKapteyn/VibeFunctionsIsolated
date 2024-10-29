namespace VibeFunctionsIsolated.Models.Interfaces;

internal interface ISquareCatalogItem
{
    public string Id { get; set; }
    public string Name { get; set; } 
    public string? Description { get; set; } 
    public string? ImageURL { get; set; } 
}

