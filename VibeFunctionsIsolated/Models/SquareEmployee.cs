using Square.Models;
using VibeFunctionsIsolated.Enums;

namespace VibeCollectiveFunctions.Models
{
    internal class SquareEmployee(CatalogObject item, List<CatalogCustomAttributeValue> customAttributes, string imageURL)
    {
        public string Id { get; set; } =  item.Id;
        public string Name { get; set; } = item.ItemData.Name;
        public string Description { get; set; } = item.ItemData.Description;
        public string? ImageURL { get; set; } = imageURL;
        public string? Sign { get; set; } = getAttribute(CustomAttributes.Sign, customAttributes);
        public string? Email { get; set; } = getAttribute(CustomAttributes.Email, customAttributes);
        public string? ThreeWordsToDescribe { get; set; } = getAttribute(CustomAttributes.Describe, customAttributes);
        public string? Expertise { get; set; } = getAttribute(CustomAttributes.Expertise, customAttributes);

        private static string? getAttribute(string attributeName, List<CatalogCustomAttributeValue> values)
        {
            CatalogCustomAttributeValue? attribute = values.Where(attr => attr.Name == attributeName).FirstOrDefault();
            
            return attribute?.StringValue;
        }
    }
}
