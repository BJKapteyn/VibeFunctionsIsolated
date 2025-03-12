using System.Text.Json.Serialization;

namespace VibeFunctionsIsolated.Models.Cosmos;

[JsonSerializable(typeof(CalendarEvent))]
public class CalendarEvent
{
    public CalendarEvent(string eventId, string name, string? description, DateTime startDate, DateTime? endDate)
    {
        EventId = eventId;
        EventName = name;
        EventDescription = description;
        StartDate = startDate;
        EndDate = endDate;
    }

    [JsonPropertyName("eventId")]
    public string EventId { get; set; }
    [JsonPropertyName("eventName")]
    public string EventName { get; set; }
    [JsonPropertyName("eventDescription")]
    public string? EventDescription { get; set; }
    [JsonPropertyName("startDate")]
    public DateTime StartDate { get; set; }
    [JsonPropertyName("endDate")]
    public DateTime? EndDate { get; set; }
}
