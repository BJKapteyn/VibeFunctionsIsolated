using Square.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VibeCollectiveFunctions.Models
{
    internal class SquareItem
    {
        public SquareItem() { }
        public SquareItem(CatalogObject item)
        {
            Name = item.ItemData.Name;
            Description = item.ItemData.Description;
            Variations = setVariations(item);
        }

        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public List<SquareItem>? Variations { get; set; }

        private static List<SquareItem>? setVariations(CatalogObject item)
        {
            if (item.ItemData.Variations == null || 
                item.ItemData.Variations.Count == 0) 
                return null;

            List<SquareItem> variations = item.ItemData.Variations
                .Select(variation => 
                {
                    SquareItem item = new SquareItem()
                    {
                        Name = variation.ItemVariationData.Name
                    };

                    return item;
                    
                }).ToList();

            return variations;
        }
    }
}
