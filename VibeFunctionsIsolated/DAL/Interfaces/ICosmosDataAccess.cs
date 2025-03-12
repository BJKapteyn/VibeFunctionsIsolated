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
    Task<T> DeleteItemAsync<T>(string id);
    Task<T?> GetItemAsync<T>(string id);
    Task<IEnumerable<T>> GetItemsAsync<T>(string query);

    /// <summary>
    /// Insert an item into the container
    /// </summary>
    /// <typeparam name="T">Type of model to be inserted</typeparam>
    /// <param name="id">Id of item to be updated if it already exists</param>
    /// <param name="item">Model of item to be upserted</param>
    /// <returns>Task containing the model for item inserted or updated</returns>
    Task<T> UpsertItemAsync<T>(T item, string? id = null);
}