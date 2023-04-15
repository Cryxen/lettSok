using System;
using Newtonsoft.Json;
namespace BlazorView.Data

{
	public class FetchLocationsFromDb
	{
        private static HttpClient client = new HttpClient();

        public async Task<string> FetchLocations()
        {
            string json = await client.GetStringAsync("https://localhost:7293/V5UserPreferencesDatabase/getLocations");

            return json;
        }

        public async Task<string> FetchPreferredLocations()
        {
            string json = await client.GetStringAsync("https://localhost:7293/V5UserPreferencesDatabase/getSearchLocations");

            return json;
        }

        public async void PostPreferredLocation(PreferredLocation preferredLocation)
        {
            var body = JsonConvert.SerializeObject(preferredLocation);
            StringContent content = new StringContent(body, encoding: System.Text.Encoding.UTF8, "application/json");
            using var response = await client.PostAsync("https://localhost:7293/V5UserPreferencesDatabase/saveSearchLocation", content);
        }

    }
}

