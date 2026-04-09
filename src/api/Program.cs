using Azure.Data.Tables;
using Azure.Identity;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddSingleton(sp =>
        {
            var connectionString = Environment.GetEnvironmentVariable("TABLE_CONNECTION_STRING");
            if (!string.IsNullOrEmpty(connectionString))
            {
                return new TableServiceClient(connectionString);
            }

            var tableServiceUri = Environment.GetEnvironmentVariable("TABLE_SERVICE_URI")
                ?? throw new InvalidOperationException("TABLE_SERVICE_URI or TABLE_CONNECTION_STRING must be configured.");
            return new TableServiceClient(new Uri(tableServiceUri), new DefaultAzureCredential());
        });
    })
    .Build();

host.Run();
