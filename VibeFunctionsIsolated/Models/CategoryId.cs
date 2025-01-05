namespace VibeFunctionsIsolated.Models;

public class CategoryId 
{
    public CategoryId(string id, string? productType) 
    {
        Id = id;
        ProductType = productType;
    }
    public CategoryId(string id) : this(id, null)
    {

    }

    public string Id { get; set; }
    public string? ProductType { get; set; }
}
