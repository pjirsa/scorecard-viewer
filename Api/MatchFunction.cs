using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BlazorApp.Shared;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Api
{
    public class MatchFunction
    {
        private readonly ILogger _logger;
        private readonly CosmosClient _dbClient;

        public MatchFunction(ILoggerFactory loggerFactory, CosmosClient dbClient)
        {
            _logger = loggerFactory.CreateLogger<MatchFunction>();
            _dbClient = dbClient;
        }

        [Function("ListMatches")]
        public async Task<HttpResponseData> GetAll([HttpTrigger(AuthorizationLevel.Function, "get", Route = "matches")] HttpRequestData req)
        {
            _logger.LogInformation("Fetching all matches.");

            var matchDb = _dbClient.GetContainer("TuesdayLeague", "Schedule");  // TODO: inject using DI TOptions
            var query = new QueryDefinition(
                query: "SELECT * FROM c"
            );
            
            var results = new List<Match>();
            
            var feed = matchDb.GetItemQueryIterator<Match>(query);

            while (feed.HasMoreResults)
            {
                foreach(var match in await feed.ReadNextAsync())
                {
                    results.Add(match);
                }
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(results.OrderBy(m => m.TeeTime));

            return response;
        }
    }
}
