using System.Text.Json.Serialization;

namespace VibeFunctionsIsolated.Models.Square;

public class GetItemByIdRequestProps
{
    public GetItemByIdRequestProps(string categoryId)
    {
        CategoryIds = [categoryId];
    }

    [JsonPropertyName("category_ids")]
    public List<string> CategoryIds { get; set; }
}
