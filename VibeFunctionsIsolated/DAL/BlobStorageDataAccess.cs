using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace VibeFunctionsIsolated.DAL;

public class BlobStorageDataAccess
{
    private readonly BlobServiceClient blobServiceClient;

    public BlobStorageDataAccess(BlobServiceClient blobServiceClient)
    {
        this.blobServiceClient = blobServiceClient;
    }

    public string UploadBlob(Stream imageStream)
    {
        var containerClient = blobServiceClient.GetBlobContainerClient("vibeimages");
        containerClient.CreateIfNotExists();
        Response<BlobContentInfo> r = containerClient.UploadBlob("testblob", imageStream);

        string hostURL = r.Value.ToString();
        return "";
    }
}
