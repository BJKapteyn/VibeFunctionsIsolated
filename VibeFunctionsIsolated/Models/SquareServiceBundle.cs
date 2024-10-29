using VibeCollectiveFunctions.Models;

namespace VibeFunctionsIsolated.Models
{
    internal class SquareServiceBundle(SquareCategory category, IEnumerable<SquareItem> squareItems)
    {
        public SquareCategory Category { get; set; } = category;
        public IEnumerable<SquareItem> Items { get; set; } = squareItems;
    }
}
