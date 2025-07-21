using Microsoft.Extensions.Logging;
using System.Text.Json;
using VibeFunctionsIsolated.Utility.UtilityInterfaces;

namespace VibeFunctionsIsolated.Utility;

/// <summary>
/// General inner application utility methods
/// </summary>
/// <param name="logger">Logger for information and errors</param>
public class ApplicationUtility(ILogger<ApplicationUtility> logger) : IApplicationUtility
{
    private readonly ILogger<ApplicationUtility> logger = logger;

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
}
