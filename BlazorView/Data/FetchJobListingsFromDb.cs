using System;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace BlazorView.Data
{
	public class FetchJobListingsFromDb
	{
        private readonly ILogger _logger;

        public FetchJobListingsFromDb(ILogger<FetchJobListingsFromDb> logger)
        {
            this._logger = logger;
        }

        private static HttpClient s_client = new HttpClient();
        
        public async Task<string> FetchJobListings()
		{
            string Json = "";
            try
            {
                _logger.LogDebug("Fetching Job Listings from Database, time: {time}", DateTimeOffset.Now);

                Json = await s_client.GetStringAsync("https://localhost:7223/api/V2/Advertisements/getAdvertisements");
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Can't fetch advertisements from database, time: {time}", DateTimeOffset.Now);
            }

            return Json;
        }
	}
}

