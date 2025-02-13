namespace VibeFunctionsIsolated.Models.Cosmos;

public class Event
{
    public string EventId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
