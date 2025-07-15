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

    public string id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public DateTime PublishDate { get; set; }
    public string Content { get; set; }
    public string? ImageUrl { get; set; }
}
