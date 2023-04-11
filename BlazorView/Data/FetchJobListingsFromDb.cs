using System;
using System.Net.Http.Json;

namespace BlazorView.Data
{
	public class FetchJobListingsFromDb
	{
        private static HttpClient client = new HttpClient();

        public async Task<String> FetchJobListings()
		{

            string json = await client.GetStringAsync("https://localhost:7223/api/V2/Advertisements/getAdvertisements");

            return json;

     
        }
	}
}

