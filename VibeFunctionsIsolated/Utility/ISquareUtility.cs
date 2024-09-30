using Square;

namespace VibeCollectiveFunctions.Utility
{
    internal interface ISquareUtility
    {
        public SquareClient InitializeClient();
        public T? Deserialize<T>(string json);
    }
}
