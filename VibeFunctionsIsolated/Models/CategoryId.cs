namespace VibeFunctionsIsolated.Models;

public class CategoryId 
{
<<<<<<< Updated upstream
    [JsonSerializable(typeof(CategoryId))]
    internal class CategoryId
=======
    public CategoryId(string id, string? productType) 
>>>>>>> Stashed changes
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
