using System.Text.Json.Serialization;

namespace VibeFunctionsIsolated.Models.Square
{
    //[JsonSerializable(typeof(GetItemByIdRequestProps))]
    public class GetItemByIdRequestProps
    {
        public GetItemByIdRequestProps(string categoryId)
        {
            CategoryIds = [categoryId];
        }

    [JsonPropertyName("category_ids")]
    public List<string> CategoryIds { get; set; }
}
