using Microsoft.Azure.Cosmos;

namespace VibeFunctionsIsolated.Models.Interfaces
{
    public interface IVibeCosmosItem
    {
        #pragma warning disable IDE1006 // Naming Styles
        public string id { get; set; }
        #pragma warning restore IDE1006 // Naming Styles
        public PartitionKey PartitionKey { get; set; }
    }
}
