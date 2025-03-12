using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using VibeFunctionsIsolated.DAL.Interfaces;

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
        string? cosmosKey = System.Environment.GetEnvironmentVariable("CosmosDBKey");
        string? cosmosEndpoint = System.Environment.GetEnvironmentVariable("CosmosDBEndpoint");
        string? comsosDBId = System.Environment.GetEnvironmentVariable("CosmosDBId");
        

        if (string.IsNullOrEmpty(cosmosKey) || string.IsNullOrEmpty(cosmosEndpoint) || string.IsNullOrEmpty(comsosDBId))
        {
            const string errorMessage = "CosmosDBKey or CosmosDBEndpoint is not set in the environment variables";
            logger.LogError(message: errorMessage);

            throw new ArgumentNullException(cosmosKey, errorMessage);
        }

        cosmosClient = new CosmosClient(cosmosEndpoint, cosmosKey);
        container = cosmosClient.GetContainer(comsosDBId, "Events");
    }

    public void ChangeContainerName(string containerName)
    {
        container = cosmosClient.GetContainer(container.Database.Id, containerName);
    }

    public async Task<IEnumerable<T>> GetItemsAsync<T>(string query)
    {
        List<T> items = [];
        QueryDefinition queryDefinition = new(query);
        cosmosClient.GetContainer(container.Database.Id, container.Id);
        FeedIterator<T> feedIterator = container.GetItemQueryIterator<T>(queryDefinition);
        while (feedIterator.HasMoreResults)
        {
            FeedResponse<T> response = await feedIterator.ReadNextAsync();
            items.AddRange(response);
        }

        return items;
    }

    public async Task<T?> GetItemAsync<T>(string id)
    {
        try
        {
            ItemResponse<T> response = await container.ReadItemAsync<T>(id, new PartitionKey(id));

            return response.Resource;
        }
        catch (CosmosException ex)
        {
            logger.LogError(ex, "Error getting item from CosmosDB");
        }

        return default;
    }

    public async Task<T> UpsertItemAsync<T>(T item, string? updateItemId = null)
    {
        ItemResponse<T> response = await container.UpsertItemAsync(item);

        if (response.StatusCode != System.Net.HttpStatusCode.OK || response.StatusCode != System.Net.HttpStatusCode.Created)
        {
            Type? upsertType = item?.GetType();
            logger.LogError("Error updating item with {updateItemId} and type {upsertType} in CosmosDB", updateItemId, upsertType);
        }

        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            logger.LogInformation("Item with {updateItemId} updated in CosmosDB", updateItemId);
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.Created)
        {
            logger.LogInformation("Item with {updateItemId} created in CosmosDB", updateItemId);
        }

        return response.Resource;
    }

    public async Task<T> DeleteItemAsync<T>(string id)
    {
        ItemResponse<T> item = await container.DeleteItemAsync<T>(id, new PartitionKey(id));

        return item;
    }
}
