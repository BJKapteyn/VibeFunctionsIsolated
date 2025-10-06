using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VibeFunctionsIsolated.Models.Square
{
    public class CatalogItemIds
    {
        public string ItemId { get; set; }
        public IEnumerable<string> VariationIds { get; set; }
    }
}
