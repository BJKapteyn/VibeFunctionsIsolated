using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using VibeFunctionsIsolated.DAL.Interfaces;
using VibeFunctionsIsolated.Models.Interfaces;

namespace VibeFunctionsIsolated.DAL;

/// <summary>
/// Data access class for CosmosDB containers, default container is Events
/// </summary>
public class CosmosDataAccess : ICosmosDataAccess
{
    private readonly ILogger<CosmosDataAccess> logger;
    private readonly CosmosClient cosmosClient;
    private Container container;

    public CosmosDataAccess(ILogger<CosmosDataAccess> logger)
    {
        this.logger = logger;
        string? cosmosKey = Environment.GetEnvironmentVariable("CosmosDBKey");
        string? cosmosEndpoint = Environment.GetEnvironmentVariable("CosmosDBEndpoint");
        string? comsosDBId = Environment.GetEnvironmentVariable("CosmosDBId");
        

        if (string.IsNullOrEmpty(cosmosKey) || string.IsNullOrEmpty(cosmosEndpoint) || string.IsNullOrEmpty(comsosDBId))
        {
            const string errorMessage = "CosmosDBKey or CosmosDBEndpoint is not set in the environment variables";
            logger.LogError(message: errorMessage);

            throw new ArgumentNullException(cosmosKey, errorMessage);
        }

        // needs to be changed to a singleton
        cosmosClient = new CosmosClient(cosmosEndpoint, cosmosKey);
        container = cosmosClient.GetContainer(comsosDBId, "Events");
    }

    public void ChangeContainerName(string containerName)
    {
        container = cosmosClient.GetContainer(container.Database.Id, containerName);
    }

    public async Task<IEnumerable<TCosmosItem>> GetAllItemsAsync<TCosmosItem>(string query) where TCosmosItem : ICosmosItem
    {
        List<TCosmosItem> items = [];
        QueryDefinition queryDefinition = new(query);

        using (cosmosClient)
        {
            cosmosClient.GetContainer(container.Database.Id, container.Id);
        }

        FeedIterator<TCosmosItem> feedIterator = container.GetItemQueryIterator<TCosmosItem>(queryDefinition);

        while (feedIterator.HasMoreResults)
        {
            FeedResponse<TCosmosItem> response = await feedIterator.ReadNextAsync();
            items.AddRange(response);
        }

        return items;
    }

    public async Task<ICosmosItem> GetItemAsync(string id, PartitionKey partitionKey)
    {
        ItemResponse<ICosmosItem> response = await container.ReadItemAsync<ICosmosItem>(id, partitionKey);
        
        return response.Resource;
    }

    public async Task<ItemResponse<TCosmosItem>> UpsertCosmosItemAsync<TCosmosItem>(TCosmosItem cosmosItem, string? updatedItemId = null) where TCosmosItem : ICosmosItem
    {
        ItemResponse<TCosmosItem> response;

        using (cosmosClient)
        {
            response = await container.UpsertItemAsync(cosmosItem);
        }

        return response;
    }

    public async Task<T> DeleteItemAsync<T>(string id)
    {
        ItemResponse<T> item = await container.DeleteItemAsync<T>(id, new PartitionKey(id));

        return item;
    }

}
