using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
using VibeFunctionsIsolated.DAL.Interfaces;

namespace VibeFunctionsIsolated.DAL;

public class BlobStorageDataAccess : IBlobStorageDataAccess
{
    private readonly BlobServiceClient blobServiceClient;
    private readonly ILogger<BlobStorageDataAccess> logger;

    public BlobStorageDataAccess(BlobServiceClient blobServiceClient, ILogger<BlobStorageDataAccess> logger)
    {
        this.blobServiceClient = blobServiceClient;
        this.logger = logger;
    }

    public async Task<string> UploadBlob(Stream imageStream)
    {
        var containerClient = blobServiceClient.GetBlobContainerClient("vibeimages");

        await containerClient.CreateIfNotExistsAsync();

        string imageName = Guid.NewGuid().ToString() + ".jpg";
        BlobClient blobClient = containerClient.GetBlobClient(imageName);

        var headers = new BlobHttpHeaders();
        headers.ContentType = "image/jpeg";

        var validationOptions = new UploadTransferValidationOptions
        {
            ChecksumAlgorithm = StorageChecksumAlgorithm.Auto
        };

        var uploadOptions = new BlobUploadOptions()
        {
            TransferValidation = validationOptions,
            HttpHeaders = headers
        };

        Response<BlobContentInfo> r = await blobClient.UploadAsync(imageStream, uploadOptions);
        Response rawResponse = r.GetRawResponse();


        if (rawResponse.Status >= 400)
        {
            logger.LogError("Failed to upload blob to storage. Status: {Status}", rawResponse.Status);
            throw new HttpRequestException();
        }
        else
        {
            logger.LogInformation("Blob upload success with status: {Status}", rawResponse.Status);
            return blobClient.Uri.ToString();
        }
    }
}
