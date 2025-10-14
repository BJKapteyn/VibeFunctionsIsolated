namespace VibeFunctionsIsolated.DAL.Interfaces
{
    public interface IBlobStorageDataAccess
    {
        /// <summary>
        /// Upload a blob to storage and return the URL
        /// </summary>
        /// <param name="imageStream">Stream to forward to Azure Blob Storage</param>
        /// <returns>Host URL for the uploaded blob</returns>
        Task<string> UploadBlob(Stream imageStream);
    }
}