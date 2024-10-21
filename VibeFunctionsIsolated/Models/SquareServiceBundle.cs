using VibeCollectiveFunctions.Models;

namespace VibeFunctionsIsolated.Models
{
    internal class SquareServiceBundle(SquareItem category, IEnumerable<SquareItem> squareItems)
    {
        public SquareItem Category { get; set; } = category;
        public IEnumerable<SquareItem> Items { get; set; } = squareItems;
    }
}
