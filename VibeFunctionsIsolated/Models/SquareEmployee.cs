using Square.Models;
using VibeFunctionsIsolated.Enums;
using VibeFunctionsIsolated.Models;

namespace VibeFunctionsIsolated.Models
{
    public class SquareEmployee : SquareCatalogItem
    {
        public SquareEmployee(CatalogObject item, IEnumerable<CatalogCustomAttributeValue> customAttributes, string? imageURL) : 
            base(item.Id, item.ItemData.Name, item.ItemData.Description, imageURL)
        {
            CategoryId = item.ItemData.CategoryId;
            Sign = getCustomAttribute(CustomAttributes.Sign, customAttributes);
            Email = getCustomAttribute(CustomAttributes.Email, customAttributes);
            ThreeWordsToDescribe = getCustomAttribute(CustomAttributes.Describe, customAttributes);
            Expertise = getCustomAttribute(CustomAttributes.Expertise, customAttributes);
        }

        public string CategoryId { get; set; }
        public string? Sign { get; set; }
        public string? Email { get; set; }
        public string? ThreeWordsToDescribe { get; set; }
        public string? Expertise { get; set; } 

        private static string? getCustomAttribute(string attributeName, IEnumerable<CatalogCustomAttributeValue> values)
        {
            CatalogCustomAttributeValue? attribute = values.Where(attr => attr.Name == attributeName).FirstOrDefault();
            
            return attribute?.StringValue;
        }
    }
}
