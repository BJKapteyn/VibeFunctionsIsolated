using VibeFunctionsIsolated.Models.Interfaces;

namespace VibeFunctionsIsolated.Models.Square
{
    public class SquareCatalogItem : ISquareCatalogItem
    {
        public SquareCatalogItem(string id, string name, string description, string? imageURL)
        {
            Id = id;
            Name = name;
            Description = description;
            ImageURL = imageURL;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? ImageURL { get; set; }
    }
}
