using Microsoft.Azure.Cosmos;
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
       string? id)
    {
        this.id = id ?? Guid.NewGuid().ToString();
        EventId = eventId;
        EventName = eventName;
        EventDescription = eventDescription;
        StartDate = startDate;
        EndDate = endDate;
        PartitionKey = new PartitionKey(startDate.ToString());
    }

    public string id { get; set; }
    public PartitionKey PartitionKey { get; set; }
    public string EventId { get; set; }
    public string EventName { get; set; }
    public string? EventDescription { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
