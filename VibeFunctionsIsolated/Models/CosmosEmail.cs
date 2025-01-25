namespace VibeCollectiveIsolated.Models;

// For inbound emails from the subscribe component
public class CosmosEmail
{
    public Guid id { get; set; } = Guid.NewGuid();
    public string email { get; set; } = "";
}
