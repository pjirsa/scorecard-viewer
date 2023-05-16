using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using BlazorApp.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using System;

namespace Api
{
    public class ScorecardFunction
    {
        private readonly ILogger _logger;
        private readonly BlobServiceClient _blobService;

        public ScorecardFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ScorecardFunction>();
            _blobService = new BlobServiceClient(Environment.GetEnvironmentVariable("GalleryStorageConnection"));
        }

        [Function("GetScorecards")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "getScorecards/{weekId}")] HttpRequestData req, 
            int weekId)
        {
            var client = _blobService.GetBlobContainerClient("scorecards");
            var results = new List<Scorecard>();
            var sasBuilder = new BlobSasBuilder(BlobContainerSasPermissions.Read, System.DateTimeOffset.UtcNow.AddHours(8));

            await foreach (BlobItem item in client.GetBlobsAsync(prefix: $"Week{weekId}"))
            {
                var blob = client.GetBlobClient(item.Name);
                results.Add(new Scorecard
                {
                    WeekId = weekId,
                    Filename = item.Name,
                    ShareLinkWebUrl = blob.GenerateSasUri(sasBuilder).AbsoluteUri
                });
            }

            _logger.LogInformation($"Found {results.Count} results.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(results);

            return response;
        }
    }
}
