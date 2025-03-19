using System.Text.Json.Serialization;

namespace VibeFunctionsIsolated.Models.Square;

[JsonSerializable(typeof(GetItemByIdRequestInfo))]
public class GetItemByIdRequestInfo
{
    public GetItemByIdRequestInfo(string categoryId)
    {
        CategoryIds = [categoryId];
    }

    [JsonPropertyName("category_ids")]
    public List<string> CategoryIds { get; set; }
}
