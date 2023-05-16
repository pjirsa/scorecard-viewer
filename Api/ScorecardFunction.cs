using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using BlazorApp.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api
{
    public class ScorecardFunction
    {
        private readonly ILogger _logger;

        public ScorecardFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ScorecardFunction>();
        }

        [Function("GetScorecards")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "getScorecards/{weekId}")] HttpRequestData req, 
            int weekId)
        {
            var results = new List<Scorecard>();

            for (int i = 0; i < 5; i++)
            {
                results.Add(new Scorecard
                {
                    WeekId = weekId,
                    Filename = $"Week{weekId}Scorecard{i + 1}.jpg",
                    ShareLinkWebUrl = $"https://www.google.com/search?q=Week{weekId}Scorecard{i + 1}.jpg"
                });
            }

            _logger.LogInformation($"Found {results.Count} results.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(results);

            return response;
        }
    }
}
