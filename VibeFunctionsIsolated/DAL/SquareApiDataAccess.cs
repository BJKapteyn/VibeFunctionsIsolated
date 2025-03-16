using Microsoft.Extensions.Logging;
using System.Net;
using System.Text;
using System.Text.Json;
using VibeFunctionsIsolated.DAL.Interfaces;
using VibeFunctionsIsolated.Models.Square;

namespace VibeFunctionsIsolated.DAL;

public class SquareApiDataAccess : ISquareApiDataAccess
{
    #region Private Members

    private readonly ILogger<SquareSdkDataAccess> logger;
    private readonly HttpClient client;

    #endregion

    public SquareApiDataAccess(ILogger<SquareSdkDataAccess> logger)
    {
        this.logger = logger;
        client = new HttpClient();
    }

    public async Task<string> GetBuyNowLink(string imageId)
    {
        string buyNowLink = "";
        string getItemEndpoint = $"https://connect.squareup.com/v2/catalog/object/{imageId}";

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, getItemEndpoint);
        request.Headers.Add("Authorization", $"Bearer {System.Environment.GetEnvironmentVariable("SquareProduction")}");
        request.Headers.Add("Accept", "application/json");

        string jsonResponseBody = await GetJsonStringResponse(request);

        if (jsonResponseBody != "")
        {
            using (JsonDocument jsonBody = JsonDocument.Parse(jsonResponseBody))
            {
                List<SquareItemRawData> squareItems = new List<SquareItemRawData>();
                JsonElement root = jsonBody.RootElement;
                JsonElement squareObject;

                bool bodyHasObject = root.TryGetProperty("object", out squareObject);

                if (bodyHasObject)
                {
                    squareObject.TryGetProperty("item_data", out JsonElement itemData);
                    itemData.TryGetProperty("ecom_uri", out JsonElement ecomUriElement);

                    if(ecomUriElement.ValueKind == JsonValueKind.String)
                    {
                        buyNowLink = ecomUriElement.GetString() ?? "";
                    }
                }
            }
        }
        return buyNowLink;
    }

    public async Task<IEnumerable<SquareItemRawData>> GetSquareAPIRawData(CatalogInformation catalogInfo)
    {
        string getItemEndpoint = "https://connect.squareup.com/v2/catalog/search-catalog-items";
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, getItemEndpoint);

        request.Headers.Add("Authorization", $"Bearer {System.Environment.GetEnvironmentVariable("SquareProduction")}");
        request.Headers.Add("Accept", "application/json");

        GetItemByIdRequestInfo requestInfo = new GetItemByIdRequestInfo(catalogInfo.Id);

        request.Content = new StringContent(JsonSerializer.Serialize(catalogInfo));

        string responseJsonString = await GetJsonStringResponse(request);
         
        if (responseJsonString != "")
        {
            return new List<SquareItemRawData>();

        }

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
                    JsonElement itemData = new ();
                    item.TryGetProperty("item_data", out itemData);

                    JsonElement id = new ();
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

            return squareItems;
        }
    }

    public async Task<string> GetJsonStringResponse(HttpRequestMessage request)
    {
        HttpResponseMessage response;
        string responseBodyStr;

        try
        {
            response = await client.SendAsync(request);

            if (response.StatusCode.Equals(HttpStatusCode.OK) == false)
            {
                StringBuilder stringBuild = new StringBuilder();
                stringBuild.AppendLine(response.ReasonPhrase ?? "");
                stringBuild.AppendLine(request.Content?.ToString() ?? "");
                throw new HttpRequestException(stringBuild.ToString(), null, response.StatusCode);
            }

            responseBodyStr = await response.Content.ReadAsStringAsync();
        }
        catch (HttpRequestException ex)
        {
            string message = ex.Message;
            string className = nameof(SquareApiDataAccess);
            logger.LogError("{className} had an http error with message: {message}", className, message);

            responseBodyStr = "";
        }

        return responseBodyStr;
    }
}