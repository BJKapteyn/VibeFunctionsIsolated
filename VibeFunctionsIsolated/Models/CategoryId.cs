using System.Text.Json.Serialization;

namespace VibeFunctionsIsolated.Models
{
    [JsonSerializable(typeof(CategoryId))]
    internal class CategoryId
    {
        public CategoryId(string id) 
        {
            Id = id;
        }
        public string Id { get; set; }
    }
}
