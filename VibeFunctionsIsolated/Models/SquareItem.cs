using Square.Models;
using VibeFunctionsIsolated.Models.Interfaces;

namespace VibeFunctionsIsolated.Models;

public class SquareItem : SquareCatalogItem, ISquareCatalogItem
{
    #region Constructors

    public SquareItem(string id, string name, string price, string duration, string buyNowLink = "") : 
        base(id, name, string.Empty, string.Empty)
    {
        Price = price;
        DurationInMinutes = duration;
        BuyNowLink = buyNowLink;
    }

    public SquareItem(CatalogObject item, string? imageURL, string buyNowLink = "") : 
        base(item.Id, item.ItemData.Name, item.ItemData.Description, imageURL)
    {
        ImageURL = imageURL;
        ReportingCategoryId = item.ItemData.ReportingCategory?.Id;
        BuyNowLink = buyNowLink;

        if(item.ItemData.Variations != null)
        {
            Variations = setVariations(item);
        }
    }

    #endregion

    public string? Price { get; set; }
    public string? ReportingCategoryId { get; set; }
    public string? DurationInMinutes { get; set; }
    public string BuyNowLink { get; set; }
    public List<SquareItem>? Variations { get; set; }

    private static List<SquareItem>? setVariations(CatalogObject item)
    {
        if (item.ItemData?.Variations == null || 
            item.ItemData.Variations.Count == 0) 
            return null;

        const int millisecondsPerMinute = 60000;
        HashSet<string> hashVariationNames = new();
        List<SquareItem> variations = new ();

        foreach (CatalogObject variation in item.ItemData.Variations)
        {
            string variationName = variation.ItemVariationData.Name;
            bool isDuplicate =  hashVariationNames.TryGetValue(variationName, out string? noOut);

            if(!isDuplicate)
            {
                hashVariationNames.Add(variationName);
                long serviceDurationInMilliseconds = variation.ItemVariationData.ServiceDuration ?? 0;
                long durationInMinutes = 0;
                if(serviceDurationInMilliseconds != 0)
                {
                    durationInMinutes = (serviceDurationInMilliseconds / millisecondsPerMinute);
                }

                SquareItem itemVaration = new SquareItem(variation.ItemVariationData.ItemId,
                                                        variation.ItemVariationData.Name,
                                                        variation.ItemVariationData.PriceMoney?.Amount.ToString() ?? string.Empty,
                                                        durationInMinutes.ToString());

                variations.Add(itemVaration);
            }
        }

        return variations;
    }
}
