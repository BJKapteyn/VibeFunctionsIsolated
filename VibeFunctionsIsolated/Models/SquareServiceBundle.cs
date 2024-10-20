using VibeCollectiveFunctions.Models;

namespace VibeFunctionsIsolated.Models
{
    internal class SquareServiceBundle(string category, IEnumerable<SquareItem> squareItems)
    {
        public string CategoryName { get; set; } = category;
        public IEnumerable<SquareItem> Items { get; set; } = squareItems;
    }
}
