using VibeFunctionsIsolated.Models;
using VibeFunctionsIsolated.Models.Square;

namespace VibeFunctionsIsolated.DAL.Interfaces
{
    /// <summary>
    /// Retrieve data directly from the square API 
    /// </summary>
    public interface ISquareApiDataAccess
    {
        public Task<IEnumerable<SquareItemRawData>> GetSquareAPIRawData(CatalogInformation catalogInfo);
        /// <summary>
        /// Get the Buy Now link for a specific item
        /// </summary>
        /// <param name="id">item's Id</param>
        /// <returns>The item's buy now link if found, empty string if not</returns>
        public Task<string> GetBuyNowLink(string id);

    }
}
