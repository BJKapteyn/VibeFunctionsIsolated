using System.Text.Json.Serialization;

namespace VibeFunctionsIsolated.Models
{
    [JsonSerializable(typeof(CategoryId))]
    public class CategoryId
    {
        public CategoryId(string id) 
        {
            Id = id;
        }
        public string Id { get; set; }
    }
}
