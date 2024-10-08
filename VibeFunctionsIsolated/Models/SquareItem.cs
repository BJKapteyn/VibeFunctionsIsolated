﻿using Square.Models;
using System.Security.Permissions;

namespace VibeCollectiveFunctions.Models
{
    internal class SquareItem
    {
        public SquareItem() { }
        public SquareItem(CatalogObject item)
        {
            Name = item.ItemData.Name;
            Description = item.ItemData.Description;
            Id = item.Id;
            ImageId = item.ImageData?.Url ?? string.Empty;
            Variations = setVariations(item);
        }

        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageId {  get; set; } = string.Empty;    
        public List<SquareItem>? Variations { get; set; }

        private static List<SquareItem>? setVariations(CatalogObject item)
        {
            if (item.ItemData?.Variations == null || 
                item.ItemData.Variations.Count == 0) 
                return null;

            List<SquareItem> variations = item.ItemData.Variations
                .Select(variation => 
                {
                    SquareItem item = new SquareItem()
                    {
                        Name = variation.ItemVariationData.Name,
                        Id = variation.ItemVariationData.ItemId
                    };

                    return item;
                    
                }).ToList();

            return variations;
        }
    }
}
