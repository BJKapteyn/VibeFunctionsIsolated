using Microsoft.Azure.Cosmos;

namespace VibeFunctionsIsolated.DAL.Interfaces
{
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
        Task<T> UpsertItemAsync<T>(string id, T item);
    }
}