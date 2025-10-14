using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using VibeFunctionsIsolated.DAL.Interfaces;

namespace VibeFunctionsIsolated.DAL;

public class BlobStorageDataAccess : IBlobStorageDataAccess
{
    private readonly BlobServiceClient blobServiceClient;

    public BlobStorageDataAccess(BlobServiceClient blobServiceClient)
    {
        this.blobServiceClient = blobServiceClient;
    }

    public async Task<string> UploadBlob(Stream imageStream)
    {
        var containerClient = blobServiceClient.GetBlobContainerClient("vibeimages");
        containerClient.CreateIfNotExists();
        Response<BlobContentInfo> r = await containerClient.UploadBlobAsync("testblob", imageStream);
        string hostURL = r.Value.ToString() ?? "";

        return hostURL;
    }
}
