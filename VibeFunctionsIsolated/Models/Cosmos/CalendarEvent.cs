using System.Security.Policy;
using System.Text.Json.Serialization;
using VibeFunctionsIsolated.Models.Interfaces;

namespace VibeFunctionsIsolated.Models.Cosmos;

[JsonSerializable(typeof(CalendarEvent))]
public class CalendarEvent : ICosmosItem
{
    public CalendarEvent(
       string eventId,
       string eventName,
       string? eventDescription,
       DateTime startDate,
       DateTime? endDate,
       string? eventOrganizerName,
       string? bannerImageUrl,
       string? id)
    {
        this.id = id ?? Guid.NewGuid().ToString();
        EventId = eventId;
        EventName = eventName;
        EventDescription = eventDescription;
        StartDate = startDate;
        EndDate = endDate;
        EventOrganizerName = eventOrganizerName ?? "";
        BannerImageUrl = bannerImageUrl ?? "";
    }

    public string id { get; set; }
    public string EventId { get; set; }
    public string EventName { get; set; }
    public string? EventDescription { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string EventOrganizerName { get; set; }
    public string BannerImageUrl { get; set; }
}
