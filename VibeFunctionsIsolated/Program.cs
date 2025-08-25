using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VibeFunctionsIsolated.Utility;
using VibeFunctionsIsolated.DAL;
using VibeFunctionsIsolated.DAL.Interfaces;
using VibeFunctionsIsolated.Utility.UtilityInterfaces;
using Microsoft.Azure.Cosmos;


var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.AddHttpClient();
        services.ConfigureFunctionsApplicationInsights();
        services.AddSingleton<ISquareUtility, SquareDalUtility>();
        services.AddSingleton<ICalendarEventUtility, CalendarEventUtility>();
        services.AddScoped<ISquareSdkDataAccess, SquareSdkDataAccess>();
        services.AddScoped<ISquareApiDataAccess, SquareApiDataAccess>();
        services.AddScoped<ICosmosDataAccess, CosmosDataAccess>();
        services.AddScoped<IApplicationUtility, ApplicationUtility>();
        services.AddScoped<IBlogPostUtility, BlogPostUtility>();
    })
    .Build();

host.Run();
