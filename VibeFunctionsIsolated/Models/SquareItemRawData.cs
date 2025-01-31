using System.Text.Json.Serialization;
using VibeFunctionsIsolated.Models.Interfaces;

namespace VibeFunctionsIsolated.Models
{
    [JsonSerializable(typeof(SquareItemRawData))]
    public class SquareItemRawData : ISquareCatalogItem

    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("ecom_uri")]
        public string? BuyNowUrl { get; set; }
        public string? ImageUrl { get; set; }
    }
}
