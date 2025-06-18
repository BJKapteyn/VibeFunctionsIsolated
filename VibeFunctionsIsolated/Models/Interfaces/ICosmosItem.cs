namespace VibeFunctionsIsolated.Models.Interfaces
{
    public interface ICosmosItem
    {
        public string id { get; set; }
        public string PartitionKey { get; set; }
    }
}
