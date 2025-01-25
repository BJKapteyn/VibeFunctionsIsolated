using System.Text.Json.Serialization;

namespace VibeFunctionsIsolated.Models
{
    [JsonSerializable(typeof(SquareItemRawData))]
    public class SquareItemRawData
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [JsonPropertyName("ecom_uri")]
        public string? BuyNowUrl { get; set; }
    }
}
