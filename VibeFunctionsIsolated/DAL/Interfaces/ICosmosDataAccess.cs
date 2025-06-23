using Microsoft.Azure.Cosmos;
using VibeFunctionsIsolated.Models.Interfaces;

namespace VibeFunctionsIsolated.DAL.Interfaces;

/// <summary>
/// Interface for CosmosDB data access
/// </summary>
public interface ICosmosDataAccess
{
    /// <summary>
    /// Change the container name to desired container, call this after the constructor
    /// </summary>
    /// <param name="containerName">Name of container you want to get data from</param>
    void ChangeContainerName(string containerName);

    /// <summary>
    /// Delete an item from the container
    /// </summary>
    /// <typeparam name="T">Type of item to delete from the container</typeparam>
    /// <param name="id">Id of item to delete</param>
    /// <returns></returns>
    Task<ICosmosItem> DeleteItemAsync<ICosmosItem>(string id);

    /// <summary>
    /// Get an item from the container by id and partition key
    /// </summary>
    /// <param name="id">Guid for item to retrieve</param>
    /// <param name="partitionKey">Partition Key for the item to be retrieved</param>
    /// <returns></returns>
    Task<ICosmosItem> GetItemAsync(string id, PartitionKey partitionKey);

    /// <summary>
    /// Get all Items from the container using a query
    /// </summary>
    /// <typeparam name="ICosmosItem"></typeparam>
    /// <param name="query"></param>
    /// <returns></returns>
    Task<IEnumerable<ICosmosItem>> GetAllItemsAsync<ICosmosItem>(string query);

    /// <summary>
    /// Insert an item into the container
    /// </summary>
    /// <typeparam name="ICosmosModel">Type of cosmos item to be inserted</typeparam>
    /// <param name="id">Id of item to be updated if it already exists</param>
    /// <param name="item">Model of item to be upserted</param>
    /// <returns>Task containing the model for item inserted or updated</returns>
    Task<ItemResponse<TCosmosItem>> UpsertCosmosItemAsync<TCosmosItem>(TCosmosItem item, string? updatedItemId = null) where TCosmosItem : ICosmosItem;
}