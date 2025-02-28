namespace VibeFunctionsIsolated.Models.Cosmos;

public class Event
{
    public Event(string eventId, string name, string description, DateTime startDate, DateTime endDate)
    {
        EventId = eventId;
        EventName = name;
        Description = description;
        StartDate = startDate;
        EndDate = endDate;
    }

    public string EventId { get; set; }
    public string EventName { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
