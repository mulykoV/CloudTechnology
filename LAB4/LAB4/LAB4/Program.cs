using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Azure.Storage.Blobs;
using Azure.AI.TextAnalytics;
using Azure;

namespace LAB4
{
    internal class Program
    {
        static void Main(string[] args)
        {
            FunctionsDebugger.Enable();

            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices(services =>
                {
                    string blobConnectionString = Environment.GetEnvironmentVariable("blobConn");
                    services.AddSingleton(new BlobServiceClient(blobConnectionString));

                    string endpoint = Environment.GetEnvironmentVariable("textAnalyticsEndpoint");
                    string key = Environment.GetEnvironmentVariable("textAnalyticsKey");
                    var credentials = new AzureKeyCredential(key);
                    services.AddSingleton(new TextAnalyticsClient(new Uri(endpoint), credentials));
                })
                .Build();

            host.Run();
        }
    }
}