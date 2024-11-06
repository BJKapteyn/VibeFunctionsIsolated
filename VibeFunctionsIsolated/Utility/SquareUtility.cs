using Microsoft.Extensions.Logging;
using Square;
using Square.Authentication;
using Square.Models;
using System.Text.Json;

namespace VibeCollectiveFunctions.Utility
{
    internal class SquareUtility : ISquareUtility
    {
        public async Task<T?> DeserializeStream<T>(Stream body)
        {
            T? deserializedJson;
            using (StreamReader reader = new(body))
            {
                string streamText = await reader.ReadToEndAsync();
                deserializedJson = JsonSerializer.Deserialize<T>(streamText);
            };

            return deserializedJson;
        }

        public SquareClient InitializeClient()
        {
            BearerAuthModel bearerAuth = new BearerAuthModel.Builder(System.Environment.GetEnvironmentVariable("SquareProduction")).Build();
            SquareClient client = new SquareClient.Builder()
                .Environment(Square.Environment.Production)
                .BearerAuthCredentials(bearerAuth)
                .Build();

            return client;
        }

        public string GetImageURL(string? imageId, SquareClient client, ILogger logger)
        {
            if (imageId == null)
                return "";

            string? result;
            CatalogObject? item;

            try
            {
                item = client.CatalogApi.RetrieveCatalogObject(imageId)?.MObject;
                result = item?.ImageData?.Url ?? "";
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                result = "";
            }

            return result;
        }
    }
}
