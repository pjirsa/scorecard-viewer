using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Azure.Functions.Worker.Configuration;
using Microsoft.Extensions.Azure;
using System;
using Azure.Identity;

namespace ApiIsolated
{
    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                //.ConfigureFunctionsWorkerDefaults(builder =>
                //{                    
                //    //builder.Services.AddAzureClients(clientBuilder =>
                //    //{
                //    //    clientBuilder.AddBlobServiceClient(Environment.GetEnvironmentVariable("galleryStorageUrl"));

                //    //    clientBuilder.UseCredential(new DefaultAzureCredential(new DefaultAzureCredentialOptions { TenantId = Environment.GetEnvironmentVariable("AzureTenantId")}));
                //    //});
                //})
                
                .Build();

            host.Run();
        }
    }
}