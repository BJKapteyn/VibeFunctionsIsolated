namespace VibeFunctionsIsolated.Models.Square
{
    public class SquareServiceBundle(SquareCategory category, IEnumerable<SquareItem> squareItems)
    {
        public SquareCategory Category { get; set; } = category;
        public IEnumerable<SquareItem> Items { get; set; } = squareItems;
    }
}
