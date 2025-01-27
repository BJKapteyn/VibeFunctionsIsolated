using Microsoft.Extensions.Logging;
using Square;
using System.Net;
using System.Text.Json;
using VibeFunctionsIsolated.Models;

namespace VibeFunctionsIsolated.DAL;

public class SquareApiDataAccess : SquareDataAcess
{
    #region Private Members

    private readonly ILogger<SquareSdkDataAccess> logger;
    private SquareClient squareClient { get; }

    #endregion

    public SquareApiDataAccess(ILogger<SquareSdkDataAccess> logger)
    {
        this.logger = logger;
        squareClient = InitializeClient();
    }

    public async Task<IEnumerable<SquareItemRawData>> GetSquareAPIRawData(CatalogInformation catalogInfo)
    {
        string getItemEndpoint = "https://connect.squareup.com/v2/catalog/search-catalog-items";
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, getItemEndpoint);

        request.Headers.Add("Authorization", $"Bearer {System.Environment.GetEnvironmentVariable("SquareProduction")}");
        request.Headers.Add("Accept", "application/json");
        request.Content = new StringContent(JsonSerializer.Serialize(catalogInfo));

        string responseJsonString = await getJsonStringResponse(request);

        if (responseJsonString != "")
        {
            using (JsonDocument jsonBody = JsonDocument.Parse(responseJsonString))
            {
                List<SquareItemRawData> squareItems = new List<SquareItemRawData>();
                JsonElement root = jsonBody.RootElement;
                JsonElement items;
                bool hasItemsProperty = root.TryGetProperty("items", out items);
                if (hasItemsProperty)
                {
                    foreach (JsonElement item in items.EnumerateArray())
                    {
                        JsonElement itemData = new();
                        item.TryGetProperty("item_data", out itemData);

                        JsonElement id = new();
                        item.TryGetProperty("id", out id);

                        string itemId = id.GetString() ?? "";

                        SquareItemRawData? squareItem = JsonSerializer.Deserialize<SquareItemRawData>(itemData);

                        if (squareItem != null)
                        {
                            squareItem.Id = itemId;
                            squareItems.Add(squareItem);
                        }
                    }
                }
            }
        }

        return new List<SquareItemRawData>();
    }

    #region Private Methods
    private async Task<string> getJsonStringResponse(HttpRequestMessage request)
    {
        HttpClient client = new HttpClient();
        HttpResponseMessage response;
        string responseBodyStr;

        try
        {
            response = await client.SendAsync(request);

            if (response.StatusCode.Equals(HttpStatusCode.OK) == false)
            {
                string message = response.ReasonPhrase ?? "";
                throw new HttpRequestException(message, null, response.StatusCode);
            }

            responseBodyStr = await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            string message = ex.Message;
            logger.LogError("{message}", message);

            responseBodyStr = "";
        }

        return responseBodyStr;
    }
    #endregion
}