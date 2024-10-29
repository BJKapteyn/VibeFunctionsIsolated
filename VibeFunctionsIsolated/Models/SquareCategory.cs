using Square.Models;

namespace VibeFunctionsIsolated.Models
{
    internal class SquareCategory : SquareCatalogItem
    {
        public SquareCategory (CatalogObject category, string imageUrl) : base(category.Id, category.CategoryData.Name, string.Empty, imageUrl)
        {
        }
    }
}
