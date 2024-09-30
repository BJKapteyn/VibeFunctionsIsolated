namespace VibeCollectiveFunctions.Models
{
    // For inbound emails from the subscribe component
    internal class CosmosEmail
    {
        public Guid id { get; set; } = Guid.NewGuid();
        public string email { get; set; } = "";
    }
}
