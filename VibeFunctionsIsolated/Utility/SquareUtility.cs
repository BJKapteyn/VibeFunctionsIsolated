using Square;
using Square.Authentication;
using System.Text.Json;

namespace VibeCollectiveFunctions.Utility
{
    internal class SquareUtility : ISquareUtility
    {
        public T? Deserialize<T>(string json)
        {
            T? deserializedJson;
            using (StreamReader reader = new(json))
            {
                string streamText = reader.ReadToEnd();
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
    }
}
