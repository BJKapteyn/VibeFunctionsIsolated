using Microsoft.Azure.Cosmos;

namespace VibeFunctionsIsolated.DAL.Interfaces
{
    public interface ICosmosDataAccess
    {
        void ChangeContainerName(string containerName);
        Task<T> DeleteItemAsync<T>(string id);
        Task<T?> GetItemAsync<T>(string id);
        Task<IEnumerable<T>> GetItemsAsync<T>(string query);
        Task<T> UpsertItemAsync<T>(string id, T item);
    }
}