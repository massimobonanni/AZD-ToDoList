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
            var tableServiceUri = Environment.GetEnvironmentVariable("TABLE_SERVICE_URI");
            var tableServiceConnectionString = Environment.GetEnvironmentVariable("TABLE_SERVICE_CONNECTIONSTRING");
            
            if (string.IsNullOrEmpty(tableServiceUri) && string.IsNullOrEmpty(tableServiceConnectionString))
                throw new InvalidOperationException("Both TABLE_SERVICE_URI and TABLE_SERVICE_CONNECTIONSTRING are not configured.");

            // if tableServiceUri is not set, use the connection string to create the client
            if (string.IsNullOrEmpty(tableServiceUri))
                return new TableServiceClient(tableServiceConnectionString);

            var uri = new Uri(tableServiceUri);
            return new TableServiceClient(uri, new DefaultAzureCredential());
        });
    })
    .Build();

host.Run();
