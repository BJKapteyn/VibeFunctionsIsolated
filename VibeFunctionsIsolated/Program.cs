using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VibeFunctionsIsolated.Utility;
using VibeFunctionsIsolated.DAL;
using VibeFunctionsIsolated.DAL.Interfaces;
using VibeFunctionsIsolated.Utility.UtilityInterfaces;
using Microsoft.Extensions.Azure;
using Azure.Identity;

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
        services.AddSingleton<ICosmosDataAccess, CosmosDataAccess>();
        services.AddScoped<IApplicationUtility, ApplicationUtility>();
        services.AddScoped<IBlogPostUtility, BlogPostUtility>();
        services.AddScoped<IBlobStorageDataAccess, BlobStorageDataAccess>();
        services.AddAzureClients(builder =>
        {
            builder.AddBlobServiceClient(Environment.GetEnvironmentVariable("BlobStorageConnectionString"));
            DefaultAzureCredential credential = new DefaultAzureCredential();
            builder.UseCredential(credential);
        });
    })
    .Build();

host.Run();
