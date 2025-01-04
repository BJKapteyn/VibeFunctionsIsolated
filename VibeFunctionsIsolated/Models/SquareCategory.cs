using Square.Models;

namespace VibeFunctionsIsolated.Models
{
    public class SquareCategory : SquareCatalogItem
    {
        public SquareCategory(CatalogObject category, string? imageURL) : base(category.Id, category.CategoryData.Name, string.Empty, imageURL)
        {
        }
    }
}
