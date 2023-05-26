using System;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlazorView.Data
{
	public class FetchUserFromDb
	{
        private readonly ILogger _logger;

        public FetchUserFromDb(ILogger<FetchUserFromDb> logger)
        {
            this._logger = logger;
        }
        
        private static HttpClient s_client = new HttpClient();

        /// <summary>
        /// Fetch all users from database in microservice UserPreferenceDatabaseService.
        /// </summary>
        /// <returns>Json Object of all users.</returns>
        public async Task<string> FetchUser()
        {
            string Json = "";
            try
            {
                _logger.LogDebug("Fetching Users from Database, time: {time}", DateTimeOffset.Now);
                Json = await s_client.GetStringAsync("https://localhost:7293/V4UserPreferencesDatabase/getUsers");

            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Can't fetch users from Database, time: {time}", DateTimeOffset.Now);
            }
            return Json;
        }

        /// <summary>
        /// Save user to database in UserPreferenceDatabaseService.
        /// </summary>
        /// <param name="user">User object to save.</param>
        public async void PostUser(User user) 
        {
            _logger.LogDebug("Saving User: {0} to Database, time: {time}", user.Name, DateTimeOffset.Now);
            var Body = JsonConvert.SerializeObject(user);
            StringContent Content = new StringContent(Body, encoding: System.Text.Encoding.UTF8, "application/json");
            try
            {
                using var Response = await s_client.PostAsync("https://localhost:7293/V4UserPreferencesDatabase/saveUser", Content);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Can't save user {0} to Database, time: {time}", user.Name, DateTimeOffset.Now);
            }
        }

        /// <summary>
        /// Save interest to database in microservice: UserPreferenceDatabaseService.
        /// </summary>
        /// <param name="interest">Interest object to save.</param>
        public async void PostInterest(Interest interest)
        {
            _logger.LogDebug("Saving interest with advertisement Uuid: {0}, and User Id: {1} to Database, time: {time}", interest.AdvertisementUuid, interest.UserGuid, DateTimeOffset.Now);
            var Body = JsonConvert.SerializeObject(interest);
            StringContent Content = new StringContent(Body, encoding: System.Text.Encoding.UTF8, "application/json");
            using var Response = await s_client.PostAsync("https://localhost:7293/V3UserPreferencesDatabase/saveInterest", Content);
        }

        /// <summary>
        /// get all interests from database in microservice: UserPreferenceDatabaseService.
        /// </summary>
        /// <returns>JsonObject listing all interests.</returns>
        public async Task<string> FetchInterest()
        {
            string Json = "";
            try {
                _logger.LogDebug("Fetching Interests from Database, time: {time}", DateTimeOffset.Now);
                Json = await s_client.GetStringAsync("https://localhost:7293/V4UserPreferencesDatabase/getInterest");
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Can't fetch interest from Database, time: {time}", DateTimeOffset.Now);
            }
            

            return Json;
        }

        /// <summary>
        /// Delete interest from database in microservice: UserPreferenceDatabaseService.
        /// </summary>
        /// <param name="interest">Interest object to delete.</param>
        public async void DeleteInterest(Interest interest)
        {
            _logger.LogDebug("Deleting interest with Advertisement Uuid: {0}, and User Id: {1} from Database, time: {time}", interest.AdvertisementUuid, interest.UserGuid, DateTimeOffset.Now);
            string Uri = $"https://localhost:7293/V5UserPreferencesDatabase/deleteInterest?UserGuid={interest.UserGuid}&AdvertisementUuid={interest.AdvertisementUuid}";
            using var Response = await s_client.DeleteAsync(Uri);
        }

        /// <summary>
        /// Save Uninterest to database in microservice: UserPreferenceDatabaseService.
        /// </summary>
        /// <param name="interest">Interest object to save</param>
        public async void PostUninterest(Interest interest)
        {
            _logger.LogDebug("Saving Uninterest with advertisement Uuid: {0}, and User Id: {1} to Database, time: {time}", interest.AdvertisementUuid, interest.UserGuid, DateTimeOffset.Now);

            var Body = JsonConvert.SerializeObject(interest);
            StringContent Content = new StringContent(Body, encoding: System.Text.Encoding.UTF8, "application/json");
            using var Response = await s_client.PostAsync("https://localhost:7293/V3UserPreferencesDatabase/saveUninterest", Content);
        }

        /// <summary>
        /// Fetch all uninterests from database in microservice: UserPreferenceDatabaseService.
        /// </summary>
        /// <returns>Json object of all uninterests in Database.</returns>
        public async Task<string> FetchUninterest()
        {
            string Json = "";

            try
            {
                _logger.LogDebug("Fetching Uninterest from Database, time: {time}", DateTimeOffset.Now);

                Json = await s_client.GetStringAsync("https://localhost:7293/V4UserPreferencesDatabase/getUninterest");
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Can't fetch uninterest from Database, time: {time}", DateTimeOffset.Now);

            }
            return Json;
        }

        /// <summary>
        /// Delete uninterest from database in microservice: UserPreferenceDatabaseService.
        /// </summary>
        /// <param name="interest">Uninterest object to delete.</param>
        public async void DeleteUninterest(Interest interest)
        {
            _logger.LogDebug("Deleting Uninterest with Advertisement Uuid: {0}, and User Id: {1} from Database, time: {time}", interest.AdvertisementUuid, interest.UserGuid, DateTimeOffset.Now);

            string Uri = $"https://localhost:7293/V5UserPreferencesDatabase/deleteUninterest?UserGuid={interest.UserGuid}&AdvertisementUuid={interest.AdvertisementUuid}";
            using var Response = await s_client.DeleteAsync(Uri);
        }
    }
}

