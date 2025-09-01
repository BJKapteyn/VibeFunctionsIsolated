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
    [JsonPropertyName("squareEventId")]
    public string SquareEventId { get; set; }
    [JsonPropertyName("eventName")]
    public string EventName { get; set; }
    [JsonPropertyName("eventDescription")]
    public string? EventDescription { get; set; }
    [JsonPropertyName("startDate")]
    public DateTime StartDate { get; set; }
    [JsonPropertyName("endDate")]
    public DateTime? EndDate { get; set; }
    [JsonPropertyName("eventOrganizerName")]
    public string EventOrganizerName { get; set; }
    [JsonPropertyName("bannerImageUrl")]
    public string BannerImageUrl { get; set; }
    [JsonPropertyName("priceInUSD")]
    public float PriceInUSD { get; set; }
}
