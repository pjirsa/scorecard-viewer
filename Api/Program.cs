using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Azure.Functions.Worker.Configuration;
using Microsoft.Extensions.Azure;
using System;
using Azure.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Cosmos;

namespace ApiIsolated
{
    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                //.ConfigureFunctionsWorkerDefaults()
                .ConfigureFunctionsWorkerDefaults(builder =>
                {
                    builder.Services.AddAzureClients(clientBuilder =>
                    {
                        clientBuilder.AddBlobServiceClient(Environment.GetEnvironmentVariable("GalleryStorageConnection"));
                    });

                    builder.Services.AddScoped(o =>
                    {
                        var cosmosConnectionString = Environment.GetEnvironmentVariable("CosmosConnectionString");
                        return new CosmosClient(cosmosConnectionString);
                    });
                })

                .Build();

            host.Run();
        }
    }
}