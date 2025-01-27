using Square;
using Square.Authentication;

namespace VibeFunctionsIsolated.DAL
{
    public abstract class SquareDataAcess
    {
        public static SquareClient InitializeClient()
        {
            BearerAuthModel bearerAuth = new BearerAuthModel.Builder(System.Environment.GetEnvironmentVariable("SquareProduction")).Build();
            SquareClient client = new SquareClient.Builder()
                .Environment(Square.Environment.Production)
                .BearerAuthCredentials(bearerAuth)
                .Build();

            return client;
        }
    }
}
