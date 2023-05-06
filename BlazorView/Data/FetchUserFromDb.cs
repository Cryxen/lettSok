using System;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlazorView.Data
{
	public class FetchUserFromDb


	{
        
        private readonly ILogger logger;

        public FetchUserFromDb(ILogger<FetchUserFromDb> logger)
        {
            this.logger = logger;
        }
        
        private static HttpClient client = new HttpClient();

        public async Task<string> FetchUser()
        {
            string json = "";
            try
            {
                logger.LogDebug("Fetching Users from Database, time: {time}", DateTimeOffset.Now);
                json = await client.GetStringAsync("https://localhost:7293/V4UserPreferencesDatabase/getUsers");

            }
            catch (Exception e)
            {
                logger.LogCritical(e, "Can't fetch users from Database, time: {time}", DateTimeOffset.Now);
            }
            return json;

        }
        /// <summary>
        /// TODO: Make handling of error codes
        /// </summary>
        /// <param name="user"></param>
        public async void PostUser(User user) 
        {
            logger.LogDebug("Saving User: {0} to Database, time: {time}", user.Name, DateTimeOffset.Now);
            var body = JsonConvert.SerializeObject(user);
            StringContent content = new StringContent(body, encoding: System.Text.Encoding.UTF8, "application/json");
            try
            {
                using var response = await client.PostAsync("https://localhost:7293/V4UserPreferencesDatabase/saveUser", content);
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "Can't save user {0} to Database, time: {time}", user.Name, DateTimeOffset.Now);
            }
        }

        public async void PostInterest(Interest interest)
        {
            logger.LogDebug("Saving interest with advertisement Uuid: {0}, and User Id: {1} to Database, time: {time}", interest.advertisementUuid, interest.userGuid, DateTimeOffset.Now);
            var body = JsonConvert.SerializeObject(interest);
            StringContent content = new StringContent(body, encoding: System.Text.Encoding.UTF8, "application/json");
            using var response = await client.PostAsync("https://localhost:7293/V3UserPreferencesDatabase/saveInterest", content);
        }

        public async Task<string> FetchInterest()
        {
            string json = "";
            try {
                logger.LogDebug("Fetching Interests from Database, time: {time}", DateTimeOffset.Now);
                json = await client.GetStringAsync("https://localhost:7293/V4UserPreferencesDatabase/getInterest");
            }
            catch (Exception e)
            {
                logger.LogCritical(e, "Can't fetch interest from Database, time: {time}", DateTimeOffset.Now);
            }
            

            return json;
        }

        public async void DeleteInterest(Interest interest)
        {
            logger.LogDebug("Deleting interest with Advertisement Uuid: {0}, and User Id: {1} from Database, time: {time}", interest.advertisementUuid, interest.userGuid, DateTimeOffset.Now);
            string uri = $"https://localhost:7293/V5UserPreferencesDatabase/deleteInterest?UserGuid={interest.userGuid}&AdvertisementUuid={interest.advertisementUuid}";
            using var response = await client.DeleteAsync(uri);
        }

        public async void PostUninterest(Interest interest)
        {
            logger.LogDebug("Saving Uninterest with advertisement Uuid: {0}, and User Id: {1} to Database, time: {time}", interest.advertisementUuid, interest.userGuid, DateTimeOffset.Now);

            var body = JsonConvert.SerializeObject(interest);
            StringContent content = new StringContent(body, encoding: System.Text.Encoding.UTF8, "application/json");
            using var response = await client.PostAsync("https://localhost:7293/V3UserPreferencesDatabase/saveUninterest", content);
        }

        public async Task<string> FetchUninterest()
        {
            string json = "";

            try
            {
                logger.LogDebug("Fetching Uninterest from Database, time: {time}", DateTimeOffset.Now);

                json = await client.GetStringAsync("https://localhost:7293/V4UserPreferencesDatabase/getUninterest");
            }
            catch (Exception e)
            {
                logger.LogCritical(e, "Can't fetch uninterest from Database, time: {time}", DateTimeOffset.Now);

            }
            return json;
        }

        public async void DeleteUninterest(Interest interest)
        {
            logger.LogDebug("Deleting Uninterest with Advertisement Uuid: {0}, and User Id: {1} from Database, time: {time}", interest.advertisementUuid, interest.userGuid, DateTimeOffset.Now);

            string uri = $"https://localhost:7293/V5UserPreferencesDatabase/deleteUninterest?UserGuid={interest.userGuid}&AdvertisementUuid={interest.advertisementUuid}";
            using var response = await client.DeleteAsync(uri);
        }
    }
}

