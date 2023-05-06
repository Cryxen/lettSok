using System;
using Newtonsoft.Json;
namespace BlazorView.Data

{
	public class FetchLocationsFromDb
	{
        private readonly ILogger logger;

        public FetchLocationsFromDb(ILogger<FetchLocationsFromDb> logger)
        {
            this.logger = logger;
        }

        private static HttpClient client = new HttpClient();

        public async Task<string> FetchLocations()
        {
            logger.LogDebug("Fetching locations from Database, time: {time}", DateTimeOffset.Now);

            string json = await client.GetStringAsync("https://localhost:7293/V5UserPreferencesDatabase/getLocations");

            return json;
        }

        public async Task<string> FetchPreferredLocations()
        {
            logger.LogDebug("Fetching Preferred locations from Database, time: {time}", DateTimeOffset.Now);

            string json = await client.GetStringAsync("https://localhost:7293/V5UserPreferencesDatabase/getSearchLocations");

            return json;
        }

        public async void PostPreferredLocation(PreferredLocation preferredLocation)
        {
            logger.LogDebug("Saving preferred location with Location Id: {0} and User Id: {1}, time: {time}", preferredLocation.LocationId, preferredLocation.UserId, DateTimeOffset.Now);
            var body = JsonConvert.SerializeObject(preferredLocation);
            StringContent content = new StringContent(body, encoding: System.Text.Encoding.UTF8, "application/json");
            using var response = await client.PostAsync("https://localhost:7293/V5UserPreferencesDatabase/saveSearchLocation", content);
        }

        public async void DeletePreferredLocation(PreferredLocation preferredLocation)
        {
            logger.LogDebug("Deleting preferred location with Location Id: {0} and User Id: {1}, time: {time}", preferredLocation.LocationId, preferredLocation.UserId, DateTimeOffset.Now);

            string uri = $"https://localhost:7293/V5UserPreferencesDatabase/deleteSearchLocation?UserId={preferredLocation.UserId}&locationId={preferredLocation.LocationId}";
            using var response = await client.DeleteAsync(uri);
        }

        public async Task<String> FetchLocationsFromInternet()
        {
            logger.LogDebug("Fetching locations from internet, time: {time}", DateTimeOffset.Now);
            string json = await client.GetStringAsync("https://localhost:7293/V5UserPreferencesDatabase/updateLocationsFromGeoNorge");
            return await FetchLocations();
        }
    }
}

