using System;
using Newtonsoft.Json;

namespace BlazorView.Data
{
	public class FetchUserFromDb


	{
		

        private static HttpClient client = new HttpClient();

        public async Task<string> FetchUser()
        {
            string json = await client.GetStringAsync("https://localhost:7293/V4UserPreferencesDatabase/getUsers");

            return json;
        }
        /// <summary>
        /// TODO: Make handling of error codes
        /// </summary>
        /// <param name="user"></param>
        public async void PostUser(User user) 
        {
            var body = JsonConvert.SerializeObject(user);
            StringContent content = new StringContent(body, encoding: System.Text.Encoding.UTF8, "application/json");
            using var response = await client.PostAsync("https://localhost:7293/V4UserPreferencesDatabase/saveUser", content);
        }

        public async void PostInterest(Interest interest)
        {
            
            var body = JsonConvert.SerializeObject(interest);
            StringContent content = new StringContent(body, encoding: System.Text.Encoding.UTF8, "application/json");
            using var response = await client.PostAsync("https://localhost:7293/V3UserPreferencesDatabase/saveInterest", content);
        }

        public async Task<string> FetchInterest()
        {
            string json = await client.GetStringAsync("https://localhost:7293/V4UserPreferencesDatabase/getInterest");

            return json;
        }

        public async void PostUninterest(Interest interest)
        {

            var body = JsonConvert.SerializeObject(interest);
            StringContent content = new StringContent(body, encoding: System.Text.Encoding.UTF8, "application/json");
            using var response = await client.PostAsync("https://localhost:7293/V3UserPreferencesDatabase/saveUninterest", content);
        }

        public async Task<string> FetchUninterest()
        {
            string json = await client.GetStringAsync("https://localhost:7293/V4UserPreferencesDatabase/getUninterest");

            return json;
        }

    }
}

