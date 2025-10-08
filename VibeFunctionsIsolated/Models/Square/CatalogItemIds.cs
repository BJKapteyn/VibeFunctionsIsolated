namespace VibeFunctionsIsolated.Models.Square;

public class CatalogItemIds
{
    public string ItemId { get; set; }
    public IEnumerable<string> VariationIds { get; set; }
    public long DatabaseVersion { get; set; }
}
