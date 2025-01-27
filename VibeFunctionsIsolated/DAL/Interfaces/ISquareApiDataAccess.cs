using VibeFunctionsIsolated.Models;

namespace VibeFunctionsIsolated.DAL.Interfaces
{
    public interface ISquareApiDataAccess
    {
        public Task<IEnumerable<SquareItemRawData>> GetSquareAPIRawData(CatalogInformation catalogInfo);
    }
}
