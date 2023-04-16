using System;
using Newtonsoft.Json;

namespace BlazorView.Data
{
	public class FetchJobListingsFromInternet
	{
        private static HttpClient client = new HttpClient();

        public async Task FetchJobListingsAndSaveToDb()
        {
            string json = await client.GetStringAsync("https://localhost:7047/api/v4/Advertisements/GetJobs");
        }

        public async Task FetchJobListingsFromLocationAndSaveToDb(string Location)
        {
            string json = await client.GetStringAsync("https://localhost:7047/api/v4/Advertisements/GetJobsByLocation?location=" + Location);

        }
    }
}

