using Square.Models;

namespace VibeFunctionsIsolated.Models
{
    internal class SquareCategory : SquareCatalogItem
    {
        public SquareCategory(CatalogObject category, string? imageURL) : base(category.Id, category.CategoryData.Name, string.Empty, imageURL)
        {
        }
    }
}
