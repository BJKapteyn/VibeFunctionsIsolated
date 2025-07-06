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

    public async Task<IEnumerable<TItem>> GetAllItemsAsync<TItem>(string query) where TItem : ICosmosItem
    {
        List<TItem> items = [];
        QueryDefinition queryDefinition = new(query);

        using (cosmosClient)
        {
            FeedIterator<TItem> feedIterator = container.GetItemQueryIterator<TItem>(queryDefinition);

            while (feedIterator.HasMoreResults)
            {
                FeedResponse<TItem> response = await feedIterator.ReadNextAsync();
                items.AddRange(response);
            }
        }

        if(items.Count == 0)
        {
            logger.LogWarning("No items found in container {ContainerName} for query: {Query}", container.Id, query);
        }
        else
        {
            logger.LogInformation("Retrieved {Count} items from container {ContainerName} for query: {Query}", items.Count, container.Id, query);
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
