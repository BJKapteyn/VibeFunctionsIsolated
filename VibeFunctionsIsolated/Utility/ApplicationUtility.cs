using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace VibeFunctionsIsolated.Utility
{
    public class ApplicationUtility(ILogger<ApplicationUtility> logger)
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
            catch(Exception ex)
            {
                logger.LogError(ex, "Failed to deserialize stream");
                deserializedJson = default;
            }
            return deserializedJson;
        }

    }
}
