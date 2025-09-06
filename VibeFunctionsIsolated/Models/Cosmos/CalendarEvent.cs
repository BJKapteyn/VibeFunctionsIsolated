using System.Security.Policy;
using System.Text.Json.Serialization;
using VibeFunctionsIsolated.Models.Interfaces;

namespace VibeFunctionsIsolated.Models.Cosmos;

[JsonSerializable(typeof(CalendarEvent))]
public class CalendarEvent : ICosmosItem
{
    public CalendarEvent(
       string? squareEventId,
       string eventName,
       string? eventDescription,
       DateTime startDate,
       DateTime? endDate,
       string? eventOrganizerName,
       string? bannerImageUrl,
       long priceInUSD,
       string? id)
    {
        this.id = id ?? Guid.NewGuid().ToString();
        // If no squareEventId is provided, the placeholder value must start with # in the Square SDK
        // and the square API will assign it an id on insert 
        SquareEventId = squareEventId ?? "#" + Guid.NewGuid().ToString();
        EventName = eventName;
        EventDescription = eventDescription;
        StartDate = startDate;
        EndDate = endDate;
        EventOrganizerName = eventOrganizerName ?? "";
        BannerImageUrl = bannerImageUrl ?? "";
        PriceInUSD = priceInUSD;
    }

    [JsonPropertyName("id")]
    public string id { get; set; }
    [JsonPropertyName("SquareEventId")]
    public string SquareEventId { get; set; }
    [JsonPropertyName("EventName")]
    public string EventName { get; set; }
    [JsonPropertyName("EventDescription")]
    public string? EventDescription { get; set; }
    [JsonPropertyName("StartDate")]
    public DateTime StartDate { get; set; }
    [JsonPropertyName("EndDate")]
    public DateTime? EndDate { get; set; }
    [JsonPropertyName("EventOrganizerName")]
    public string EventOrganizerName { get; set; }
    [JsonPropertyName("BannerImageUrl")]
    public string BannerImageUrl { get; set; }
    [JsonPropertyName("PriceInUSD")]
    public long PriceInUSD { get; set; }
}
