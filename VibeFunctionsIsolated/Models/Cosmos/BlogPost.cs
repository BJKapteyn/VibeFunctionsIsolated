using System.Text.Json.Serialization;
using VibeFunctionsIsolated.Models.Interfaces;

namespace VibeFunctionsIsolated.Models.Cosmos;

[JsonSerializable(typeof(BlogPost))]
public class BlogPost : ICosmosItem
{
    public BlogPost(
    string id,
    string title,
    string author,
    DateTime date,
    string content,
    string? imageUrl = null)
    {
        this.id = id;
        Title = title;
        Author = author;
        PublishDate = date;
        Content = content;
        ImageUrl = imageUrl;
    }

    [JsonPropertyName("id")]
    public string id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("author")]
    public string Author { get; set; }

    [JsonPropertyName("date")]
    DateTime PublishDate { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; }

    [JsonPropertyName("imageUrl")]
    public string? ImageUrl { get; set; }
}
