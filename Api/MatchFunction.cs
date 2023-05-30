using System;
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

        [Function("GetAllMatches")]
        public async Task<HttpResponseData> GetAll([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "matches")] HttpRequestData req)
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
                foreach (var match in await feed.ReadNextAsync())
                {
                    results.Add(match);
                }
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(results.OrderBy(m => m.TeeTime));

            return response;
        }

        [Function("GetMatchesByWeekId")]
        public async Task<HttpResponseData> GetByWeekId([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "matches/{weekId}")] HttpRequestData req,
            int weekId)
        {
            if (weekId == 0) throw new ArgumentException("weekId must be greater than 0", "weekId");

            _logger.LogInformation($"Fetching matches for week {weekId}");

            // Get the database and container references, then prepare the query statement
            var matchDb = _dbClient.GetContainer("TuesdayLeague", "Schedule");  // TODO: inject using DI TOptions
            var query = new QueryDefinition(
                               query: "SELECT * FROM c WHERE c.WeekId = @weekId"
                            )
                .WithParameter("@weekId", weekId);

            // Create an empty list to hold the results
            var results = new List<Match>();

            // Execute the query and iterate over the results
            var feed = matchDb.GetItemQueryIterator<Match>(query);
            while (feed.HasMoreResults)
            {
                foreach (var match in await feed.ReadNextAsync())
                {
                    results.Add(match);
                }
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(results);

            return response;
        }
    }
}
