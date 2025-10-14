using Microsoft.Extensions.Logging;
using System.Text.Json;
using VibeFunctionsIsolated.DAL.Interfaces;
using VibeFunctionsIsolated.Utility.UtilityInterfaces;

namespace VibeFunctionsIsolated.Utility;

/// <summary>
/// General inner application utility methods
/// </summary>
/// <param name="logger">Logger for information and errors</param>
public class ApplicationUtility : IApplicationUtility
{
    private readonly ILogger<ApplicationUtility> logger;
    private readonly IBlobStorageDataAccess blobStorageDataAccess;

    public ApplicationUtility(
        ILogger<ApplicationUtility> logger,
        IBlobStorageDataAccess blobStorageDataAccess)
    {
        this.logger = logger;
        this.blobStorageDataAccess = blobStorageDataAccess;
    }

    public async Task<T?> DeserializeStream<T>(Stream body)
    {
        T? deserializedJson;
        try
        {
            using (StreamReader reader = new(body))
            {
                string streamText = await reader.ReadToEndAsync();
                deserializedJson = JsonSerializer.Deserialize<T>(streamText);
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to deserialize stream");
            deserializedJson = default;
        }

        return deserializedJson;
    }

    public async Task<string> UploadImage(Stream imageStream)
    {
        // Forward the stream to blob storage and return the URL
        return await blobStorageDataAccess.UploadBlob(imageStream);
    }
}
