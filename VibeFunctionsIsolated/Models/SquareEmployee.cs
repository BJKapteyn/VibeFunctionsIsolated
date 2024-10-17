using Square.Models;
using static VibeCollectiveFunctions.Enums.SquareEnums;

namespace VibeCollectiveFunctions.Models
{
    internal class SquareEmployee(CatalogObject item, List<CatalogCustomAttributeValue> customAttributes, string imageURL)
    {
        public string Id { get; set; } =  item.Id;
        public string Name { get; set; } = item.ItemData.Name;
        public string Description { get; set; } = item.ItemData.Description;
        //SEALI67C3CY5BBD2WMB36BJA
        public string? ImageURL { get; set; } = imageURL;
        public string? Sign { get; set; } = getAttribute(CustomAttributeValues.Sign.ToString(), customAttributes);
        public string? Email { get; set; } = getAttribute(CustomAttributeValues.Email.ToString(), customAttributes);

        private static string? getAttribute(string attributeName, List<CatalogCustomAttributeValue> values)
        {
            CatalogCustomAttributeValue? attribute = values.Where(attr => attr.Name == attributeName).FirstOrDefault();
            
            return attribute?.StringValue;
        }
    }
}
