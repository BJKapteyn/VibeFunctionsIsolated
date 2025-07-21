namespace VibeFunctionsIsolated.Models.Square;

public class CatalogInformation
{
    public CatalogInformation(string id, string? productType = null, string? reportingCategoryId = null)
    {
        Id = id;
        ProductType = productType;
        ReportingCategoryId = reportingCategoryId;
    }

    public string Id { get; set; }
    public string? ReportingCategoryId { get; set; }
    public string? ProductType { get; set; }
}
