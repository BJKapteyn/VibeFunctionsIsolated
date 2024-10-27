using Square.Models;

namespace VibeCollectiveFunctions.Models
{
    internal class SquareItem
    {
        public SquareItem() { }
        public SquareItem(CatalogObject item, string imageURL)
        {
            Name = item.ItemData.Name;
            Description = item.ItemData.Description;
            Id = item.Id;
            ImageURL = imageURL;
            Variations = setVariations(item);
            ReportingCategoryId = item.ItemData.ReportingCategory?.Id;

            if(item.ItemData.Variations != null)
            {
               
            }
        }

        public SquareItem(CatalogCategory category, string imageUrl)
        {
            Name = category.Name;
            ImageURL = imageUrl;
            Id = Guid.NewGuid().ToString();
        }

        public string? ReportingCategoryId { get; set; }
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageURL {  get; set; } = string.Empty;
        public string? Price { get; set; }
        public string? DurationInMinutes { get; set; }
        public List<SquareItem>? Variations { get; set; }

        private static List<SquareItem>? setVariations(CatalogObject item)
        {
            if (item.ItemData?.Variations == null || 
                item.ItemData.Variations.Count == 0) 
                return null;
            const int millisecondsPerSecond = 1000;
            const int secondsPerMinute = 60;
            HashSet<string> hashVariationNames = new();
            List<SquareItem> variations = new ();

            foreach (CatalogObject variation in item.ItemData.Variations)
            {
                string variationName = variation.ItemVariationData.Name;
                bool isDuplicate =  hashVariationNames.TryGetValue(variationName, out string? noOut);

                if(!isDuplicate)
                {
                    hashVariationNames.Add(variationName);
                    long? durationInMinutes = ((variation.ItemVariationData.ServiceDuration / millisecondsPerSecond) / secondsPerMinute);

                    SquareItem itemVaration = new SquareItem()
                    {
                        Name = variation.ItemVariationData.Name,
                        Id = variation.ItemVariationData.ItemId,
                        Price = variation.ItemVariationData.PriceMoney?.Amount.ToString(),
                        DurationInMinutes = durationInMinutes?.ToString()
                    };
                    variations.Add(itemVaration);
                }
            }


            //List<SquareItem> variations = item.ItemData.Variations
            //    .Select(variation => 
            //    {
            //        SquareItem item = new SquareItem()
            //        {
            //            Name = variation.ItemVariationData.Name,
            //            Id = variation.ItemVariationData.ItemId,
            //            Price = variation.ItemVariationData.PriceMoney?.Amount.ToString(),
            //            DurationInMinutes = durationInMinutes?.ToString()
            //        };

            //        return item;
                    
            //    }).ToList();

            return variations;
        }
    }
}
