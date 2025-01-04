using System.Text.Json.Serialization;

namespace VibeFunctionsIsolated.Models
{
    [JsonSerializable(typeof(CategoryId))]
    internal class CategoryId
    {
        [JsonConstructor]
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
}
