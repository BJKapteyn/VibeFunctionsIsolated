using System.Text.Json.Serialization;

namespace VibeFunctionsIsolated.Models.Cosmos;

[JsonSerializable(typeof(CalendarEvent))]
public class CalendarEvent(string eventId, string name, string? description, DateTime startDate, DateTime? endDate)
{
    [JsonPropertyName("eventId")]
    public string EventId { get; set; } = eventId;
    [JsonPropertyName("eventName")]
    public string EventName { get; set; } = name;
    [JsonPropertyName("eventDescription")]
    public string? EventDescription { get; set; } = description;
    [JsonPropertyName("startDate")]
    public DateTime StartDate { get; set; } = startDate;
    [JsonPropertyName("endDate")]
    public DateTime? EndDate { get; set; } = endDate;
}
