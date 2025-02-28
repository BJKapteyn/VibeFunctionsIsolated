using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VibeFunctionsIsolated.Utility;
using VibeFunctionsIsolated.DAL;
using VibeFunctionsIsolated.DAL.Interfaces;


var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.AddHttpClient();
        services.ConfigureFunctionsApplicationInsights();
        services.AddSingleton<ISquareUtility, SquareDalUtility>();
        services.AddScoped<ISquareSdkDataAccess, SquareSdkDataAccess>();
        services.AddScoped<ISquareApiDataAccess, SquareApiDataAccess>();
        services.AddScoped<ICosmosDataAccess, CosmosDataAccess>();
    })
    .Build();

host.Run();
