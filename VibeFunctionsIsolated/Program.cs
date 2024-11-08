using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VibeCollectiveFunctions.Utility;
using VibeFunctionsIsolated.DAL;


var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.AddHttpClient();
        services.ConfigureFunctionsApplicationInsights();
        services.AddSingleton<ISquareUtility, SquareUtility>();
        services.AddScoped<ISquareDAL, SquareDAL>();
    })
    .Build();

host.Run();
