using System;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace BlazorView.Data
{
	public class FetchJobListingsFromDb
	{
        private readonly ILogger logger;

        public FetchJobListingsFromDb(ILogger<FetchJobListingsFromDb> logger)
        {
            this.logger = logger;
        }

        private static HttpClient client = new HttpClient();
        
        public async Task<string> FetchJobListings()
		{
            logger.LogDebug("Fetching Job Listings from Database, time: {time}", DateTimeOffset.Now);

            string json = await client.GetStringAsync("https://localhost:7223/api/V2/Advertisements/getAdvertisements");

            return json;
        }
	}
}

