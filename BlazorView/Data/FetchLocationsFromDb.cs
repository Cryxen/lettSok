using System;
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
    }
}

