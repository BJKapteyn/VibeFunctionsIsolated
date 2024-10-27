using Square.Models;
using VibeFunctionsIsolated.Enums;

namespace VibeCollectiveFunctions.Models
{
    internal class SquareEmployee(CatalogObject item, IEnumerable<CatalogCustomAttributeValue> customAttributes, string imageURL)
    {
        public string Id { get; set; } =  item.Id;
        public string Name { get; set; } = item.ItemData.Name;
        public string Description { get; set; } = item.ItemData.Description;
        public string? ImageURL { get; set; } = imageURL;
        public string? Sign { get; set; } = getCustomAttribute(CustomAttributes.Sign, customAttributes);
        public string? Email { get; set; } = getCustomAttribute(CustomAttributes.Email, customAttributes);
        public string? ThreeWordsToDescribe { get; set; } = getCustomAttribute(CustomAttributes.Describe, customAttributes);
        public string? Expertise { get; set; } = getCustomAttribute(CustomAttributes.Expertise, customAttributes);

        private static string? getCustomAttribute(string attributeName, IEnumerable<CatalogCustomAttributeValue> values)
        {
            CatalogCustomAttributeValue? attribute = values.Where(attr => attr.Name == attributeName).FirstOrDefault();
            
            return attribute?.StringValue;
        }
    }
}
