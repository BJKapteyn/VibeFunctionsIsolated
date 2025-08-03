using System.Text.Json;
using System.Text.Json.Serialization;
using VibeFunctionsIsolated.Models.Interfaces;

namespace VibeFunctionsIsolated.Models.Cosmos;

[JsonSerializable(typeof(BlogPost))]
public class BlogPost : ICosmosItem
{
    public BlogPost(
    string? id,
    string title,
    string author,
    DateTime publishDate,
    string content,
    string? imageUrl = null)
    {
        this.id = id ?? Guid.NewGuid().ToString();
        Title = title;
        Author = author;
        PublishDate = publishDate;
        Content = content;
        ImageUrl = imageUrl;
    }
    [JsonPropertyName("id")]
    public string id { get; set; }
    [JsonPropertyName("Title")]
    public string Title { get; set; }
    [JsonPropertyName("Author")]
    public string Author { get; set; }
    [JsonPropertyName("PublishDate")]
    public DateTime PublishDate { get; set; }
    [JsonPropertyName("Content")]
    public string Content { get; set; }
    [JsonPropertyName("ImageUrl")]
    public string? ImageUrl { get; set; }
}
