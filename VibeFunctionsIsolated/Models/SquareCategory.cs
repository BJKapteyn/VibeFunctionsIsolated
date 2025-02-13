using Square.Models;
using VibeFunctionsIsolated.Models.Interfaces;

namespace VibeFunctionsIsolated.Models;

public class SquareCategory : SquareCatalogItem, ISquareCatalogItem
{
    public SquareCategory(CatalogObject category, string? imageURL) : base(category.Id, category.CategoryData.Name, string.Empty, imageURL)
    {
    }
}
