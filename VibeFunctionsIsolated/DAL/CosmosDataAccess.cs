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

        cosmosClient = new CosmosClient(cosmosEndpoint, cosmosKey);
        container = cosmosClient.GetContainer(comsosDBId, "Events");
    }

    public void ChangeContainerName(string containerName)
    {
        container = cosmosClient.GetContainer(container.Database.Id, containerName);
    }

    public async Task<IEnumerable<ICosmosItem>> GetItemsAsync(string query)
    {
        List<ICosmosItem> items = [];
        QueryDefinition queryDefinition = new(query);
        cosmosClient.GetContainer(container.Database.Id, container.Id);
        FeedIterator<ICosmosItem> feedIterator = container.GetItemQueryIterator<ICosmosItem>(queryDefinition);
        while (feedIterator.HasMoreResults)
        {
            FeedResponse<ICosmosItem> response = await feedIterator.ReadNextAsync();
            items.AddRange(response);
        }

        return items;
    }

    public async Task<ICosmosItem> GetItemAsync(string id)
    {
        ItemResponse<ICosmosItem> response = await container.ReadItemAsync<ICosmosItem>(id, new PartitionKey(id));
        //try
        //{

        //    return response.Resource;
        //}
        //catch (CosmosException ex)
        //{
        //    logger.LogError(ex, "Error getting item from CosmosDB");
        //}

        return response.Resource;
    }

    public async Task<ICosmosItem> UpsertItemAsyncCommand(ICosmosItem cosmosItem, string? updatedItemId = null)
    {
        ItemResponse<ICosmosItem> response = await container.UpsertItemAsync(cosmosItem, cosmosItem.PartitionKey);

        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            logger.LogInformation("Item with {updateItemId} updated in CosmosDB", updatedItemId);
            // Additional logic for update success can go here
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.Created)
        {
            logger.LogInformation("Item with {updateItemId} created in CosmosDB", updatedItemId);
            // Additional logic for create success can go here
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            logger.LogWarning("Item with {updateItemId} not found in CosmosDB", updatedItemId);
            // Handle not found case
        }
        else
        {
            Type? upsertType = cosmosItem?.GetType();
            logger.LogError("Error updating item with {updateItemId} and type {upsertType} in CosmosDB. StatusCode: {statusCode}", updatedItemId, upsertType, response.StatusCode);
            // Handle other error cases
        }

        return response.Resource;
    }

    //public async Task<IVibeCosmosItem> UpsertItemAsyncCommand<IVibeCosmosItem>(IVibeCosmosItem item, string? updatedItemId = null)
    //{

    //    ItemResponse<IVibeCosmosItem> response = await container.UpsertItemAsync<IVibeCosmosItem>(item, item.PartitionKey);

    //    if (response.StatusCode != System.Net.HttpStatusCode.OK || response.StatusCode != System.Net.HttpStatusCode.Created)
    //    {
    //        Type? upsertType = item?.GetType();
    //        logger.LogError("Error updating item with {updateItemId} and type {upsertType} in CosmosDB", updatedItemId, upsertType);

    //    }

    //    if (response.StatusCode == System.Net.HttpStatusCode.OK)
    //    {
    //        logger.LogInformation("Item with {updateItemId} updated in CosmosDB", updatedItemId);
    //    }
    //    else if (response.StatusCode == System.Net.HttpStatusCode.Created)
    //    {
    //        logger.LogInformation("Item with {updateItemId} created in CosmosDB", updatedItemId);
    //    }

    //    return response.Resource;
    //}

    public async Task<T> DeleteItemAsync<T>(string id)
    {
        ItemResponse<T> item = await container.DeleteItemAsync<T>(id, new PartitionKey(id));

        return item;
    }

}
